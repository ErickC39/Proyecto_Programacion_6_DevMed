using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class EspecialidadesController : Controller
    {
        private readonly EspecialidadClient _especialidades;

        public EspecialidadesController(EspecialidadClient especialidades)
        {
            _especialidades = especialidades;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _especialidades.ListarAsync();
            return View(lista);
        }

        [HttpGet]
        public IActionResult Create() => View(new EspecialidadDto());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EspecialidadDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var r = await _especialidades.CrearAsync(model);
            if (!r.Ok)
            {
                ModelState.AddModelError("", r.Mensaje);
                return View(model);
            }

            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }
    }
}
