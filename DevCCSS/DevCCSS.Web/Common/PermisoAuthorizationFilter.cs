using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace DevCCSS.Web.Common
{
    // Conecta la tabla Roles_Permisos (hasta ahora solo se mostraba en
    // Admin > Permisos, sin afectar en nada el comportamiento real) con la
    // autorizacion de verdad. Corre ADEMAS de los [Authorize(Roles=...)]
    // que ya tiene cada controlador, nunca en su lugar: si un rol no tiene
    // fila para ese Modulo en Roles_Permisos, este filtro no restringe nada
    // (se respeta el comportamiento actual); si tiene fila y el permiso
    // puntual (Ver/Crear/Editar/Eliminar) esta en 0, se bloquea.
    public class PermisoAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private const string CacheKey = "RolesPermisos_Matriz";
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(2);

        private readonly PermisoClient _permisos;
        private readonly IMemoryCache _cache;

        public PermisoAuthorizationFilter(PermisoClient permisos, IMemoryCache cache)
        {
            _permisos = permisos;
            _cache = cache;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity?.IsAuthenticated != true)
                return; // lo maneja [Authorize] / el login

            if (context.ActionDescriptor is not ControllerActionDescriptor descriptor)
                return;

            var modulo = descriptor.ControllerTypeInfo.GetCustomAttributes(typeof(ModuloAttribute), true)
                .OfType<ModuloAttribute>()
                .FirstOrDefault()?.Nombre;
            if (modulo is null)
                return; // controlador sin modulo mapeado: sin restriccion adicional

            var accionRequerida = ResolverAccion(descriptor.ActionName);

            var roles = context.HttpContext.User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();
            if (roles.Count == 0)
                return;

            var matriz = await ObtenerMatrizAsync();

            var filas = matriz.Where(p => roles.Contains(p.NombreRol) && p.Modulo == modulo).ToList();
            if (filas.Count == 0)
                return; // ese rol no tiene fila para este modulo: no se restringe (compat. con lo existente)

            bool permitido = filas.Any(f => accionRequerida switch
            {
                Accion.Ver => f.PuedeVer,
                Accion.Crear => f.PuedeCrear,
                Accion.Editar => f.PuedeEditar,
                Accion.Eliminar => f.PuedeEliminar,
                _ => true
            });

            if (!permitido)
                context.Result = new Microsoft.AspNetCore.Mvc.ForbidResult();
        }

        private async Task<List<Contracts.RolPermisoDto>> ObtenerMatrizAsync()
        {
            if (_cache.TryGetValue(CacheKey, out List<Contracts.RolPermisoDto>? cached) && cached is not null)
                return cached;

            var matriz = await _permisos.ListarAsync();
            _cache.Set(CacheKey, matriz, CacheTtl);
            return matriz;
        }

        public enum Accion { Ver, Crear, Editar, Eliminar }

        // Heuristica por nombre de accion: cubre el patron CRUD estandar
        // (Index/Details -> Ver, Create -> Crear, Edit -> Editar,
        // Delete/DeleteConfirmed -> Eliminar) y las acciones custom de cada
        // modulo (Agendar, RegistrarLlegada, CambiarEstado, Asignar, etc.),
        // que por ser mutaciones de estado se tratan como "Editar".
        internal static Accion ResolverAccion(string actionName)
        {
            var n = actionName.ToLowerInvariant();
            if (n.Contains("delete") || n.Contains("eliminar") || n.Contains("cancelar"))
                return Accion.Eliminar;
            if (n.Contains("create") || n.Contains("crear") || n.Contains("registrar") || n.Contains("agendar") || n.Contains("solicitar"))
                return Accion.Crear;
            if (n is "index" or "details" or "detalle")
                return Accion.Ver;
            return Accion.Editar;
        }
    }
}
