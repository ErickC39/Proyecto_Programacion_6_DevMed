using DevCCSS.Web.Common;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    // Muestra la matriz de permisos por rol (tabla Roles_Permisos). Ademas de
    // ser informativa, PermisoAuthorizationFilter la lee de verdad para
    // restringir Ver/Crear/Editar/Eliminar por modulo en cada controlador
    // marcado con [Modulo("...")] -- editar esta matriz desde Admin > Permisos
    // cambia el comportamiento real del sistema (con cache de hasta 2 minutos).
    [Authorize(Roles = "Administrador")]
    [Modulo("Permisos")]
    public class PermisosController : Controller
    {
        private readonly PermisoClient _permisos;

        public PermisosController(PermisoClient permisos)
        {
            _permisos = permisos;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _permisos.ListarAsync();
            return View(lista);
        }
    }
}
