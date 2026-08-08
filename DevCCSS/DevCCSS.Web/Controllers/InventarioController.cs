using DevCCSS.Web.Common;
using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador,Facturacion")]
    [Modulo("Inventario")]
    public class InventarioController : Controller
    {
        private readonly InventarioClient _servicio;
        private readonly MedicamentoClient _medicamentos;

        public InventarioController(InventarioClient servicio, MedicamentoClient medicamentos)
        {
            _servicio = servicio;
            _medicamentos = medicamentos;
        }

        private async Task CargarMedicamentos()
        {
            ViewBag.Medicamentos = await _medicamentos.ListarAsync();
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
            await CargarMedicamentos();
            return View(new ProductoDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoDto model)
        {
            if (!ModelState.IsValid) { await CargarMedicamentos(); return View(model); }
            var r = await _servicio.CrearAsync(model);
            if (!r.Ok) { ModelState.AddModelError("", r.Mensaje); await CargarMedicamentos(); return View(model); }
            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var x = await _servicio.ObtenerPorIdAsync(id);
            if (x is null) return NotFound();
            await CargarMedicamentos();
            return View(x);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductoDto model)
        {
            if (!ModelState.IsValid) { await CargarMedicamentos(); return View(model); }
            var r = await _servicio.ActualizarAsync(model);
            if (!r.Ok) { ModelState.AddModelError("", r.Mensaje); await CargarMedicamentos(); return View(model); }
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
