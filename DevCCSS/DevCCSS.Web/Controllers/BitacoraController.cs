using DevCCSS.Web.Common;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Modulo("Bitacoras")]
    public class BitacoraController : Controller
    {
        private readonly BitacoraClient _bitacora;

        public BitacoraController(BitacoraClient bitacora)
        {
            _bitacora = bitacora;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Auditoria = await _bitacora.ListarAuditoriaAsync();
            ViewBag.Errores = await _bitacora.ListarErroresAsync();
            ViewBag.Notificaciones = await _bitacora.ListarNotificacionesAsync();
            return View();
        }
    }
}
