using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador,Medico,Enfermeria")]
    public class MedicamentosController : Controller
    {
        private readonly MedicamentoClient _medicamentos;

        public MedicamentosController(MedicamentoClient medicamentos)
        {
            _medicamentos = medicamentos;
        }

        // GET: /Medicamentos
        public async Task<IActionResult> Index()
        {
            var lista = await _medicamentos.ListarAsync();
            return View(lista);
        }

        // GET: /Medicamentos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var m = await _medicamentos.ObtenerPorIdAsync(id);
            if (m is null) return NotFound();
            return View(m);
        }

        // GET: /Medicamentos/Create
        public IActionResult Create() => View(new MedicamentoDto());

        // POST: /Medicamentos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MedicamentoDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var r = await _medicamentos.CrearAsync(model);
            if (!r.Ok)
            {
                ModelState.AddModelError("", r.Mensaje);
                return View(model);
            }
            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Medicamentos/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var m = await _medicamentos.ObtenerPorIdAsync(id);
            if (m is null) return NotFound();
            return View(m);
        }

        // POST: /Medicamentos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicamentoDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var r = await _medicamentos.ActualizarAsync(model);
            if (!r.Ok)
            {
                ModelState.AddModelError("", r.Mensaje);
                return View(model);
            }
            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Medicamentos/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var m = await _medicamentos.ObtenerPorIdAsync(id);
            if (m is null) return NotFound();
            return View(m);
        }

        // POST: /Medicamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var r = await _medicamentos.EliminarAsync(id);
            TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }
    }
}