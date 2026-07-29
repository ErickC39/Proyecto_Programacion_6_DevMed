using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    // Solo lectura: muestra la matriz de permisos por rol definida en la base de datos
    // (tabla Roles_Permisos). La autorizacion real se aplica con [Authorize(Roles=...)]
    // en cada controlador; esta pantalla es para "visualizar los permisos segun BD".
    [Authorize(Roles = "Administrador")]
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
