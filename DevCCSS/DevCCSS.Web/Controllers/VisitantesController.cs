using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador,Recepcionista")]
    public class VisitantesController : Controller
    {
        private readonly VisitanteClient _servicio;
        private readonly PacienteClient _pacientes;

        public VisitantesController(VisitanteClient servicio, PacienteClient pacientes)
        {
            _servicio = servicio;
            _pacientes = pacientes;
        }

        private async Task CargarListas()
        {
            ViewBag.Pacientes = await _pacientes.ListarAsync();
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _servicio.ListarAsync();
            return View(lista);
        }

        public async Task<IActionResult> Details(int id)
        {
            var x = await _servicio.ObtenerPorIdAsync(id);
            if (x is null) return NotFound();
            return View(x);
        }

        public async Task<IActionResult> Create()
        {
            await CargarListas();
            return View(new VisitanteDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitanteDto model)
        {
            if (!ModelState.IsValid) { await CargarListas(); return View(model); }
            var r = await _servicio.CrearAsync(model);
            if (!r.Ok) { ModelState.AddModelError("", r.Mensaje); await CargarListas(); return View(model); }
            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var x = await _servicio.ObtenerPorIdAsync(id);
            if (x is null) return NotFound();
            await CargarListas();
            return View(x);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(VisitanteDto model)
        {
            if (!ModelState.IsValid) { await CargarListas(); return View(model); }
            var r = await _servicio.ActualizarAsync(model);
            if (!r.Ok) { ModelState.AddModelError("", r.Mensaje); await CargarListas(); return View(model); }
            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var x = await _servicio.ObtenerPorIdAsync(id);
            if (x is null) return NotFound();
            return View(x);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var r = await _servicio.EliminarAsync(id);
            TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }
    }
}
