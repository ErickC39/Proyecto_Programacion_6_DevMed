using DevCCSS.Web.Common;
using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador,Medico,Enfermeria")]
    [Modulo("Enfermedades")]
    public class EnfermedadesController : Controller
    {
        private readonly EnfermedadClient _servicio;
        private readonly MedicamentoClient _medicamentos;

        public EnfermedadesController(EnfermedadClient servicio, MedicamentoClient medicamentos)
        {
            _servicio = servicio;
            _medicamentos = medicamentos;
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

            var vinculados = await _servicio.ListarMedicamentosAsync(id);
            var todos = await _medicamentos.ListarAsync();
            var idsVinculados = vinculados.Select(v => v.IdMedicamento).ToHashSet();

            ViewBag.MedicamentosVinculados = vinculados;
            ViewBag.MedicamentosDisponibles = todos.Where(m => !idsVinculados.Contains(m.IdMedicamento)).ToList();

            return View(x);
        }

        public async Task<IActionResult> Create()
        {
            return View(new EnfermedadDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EnfermedadDto model)
        {
            if (!ModelState.IsValid) { return View(model); }
            var r = await _servicio.CrearAsync(model);
            if (!r.Ok) { ModelState.AddModelError("", r.Mensaje); return View(model); }
            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var x = await _servicio.ObtenerPorIdAsync(id);
            if (x is null) return NotFound();
            return View(x);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EnfermedadDto model)
        {
            if (!ModelState.IsValid) { return View(model); }
            var r = await _servicio.ActualizarAsync(model);
            if (!r.Ok) { ModelState.AddModelError("", r.Mensaje); return View(model); }
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AsignarMedicamento(int idEnfermedad, int idMedicamento, string? observacion)
        {
            var r = await _servicio.AsignarMedicamentoAsync(idEnfermedad, idMedicamento, observacion);
            TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
            return RedirectToAction(nameof(Details), new { id = idEnfermedad });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuitarMedicamento(int idEnfermedad, int idMedicamento)
        {
            var r = await _servicio.QuitarMedicamentoAsync(idEnfermedad, idMedicamento);
            TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
            return RedirectToAction(nameof(Details), new { id = idEnfermedad });
        }
    }
}
