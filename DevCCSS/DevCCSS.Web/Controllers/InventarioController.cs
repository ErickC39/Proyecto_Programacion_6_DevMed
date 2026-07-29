using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador,Facturacion")]
    public class InventarioController : Controller
    {
        private readonly InventarioClient _servicio;

        public InventarioController(InventarioClient servicio)
        {
            _servicio = servicio;
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
            return View(new ProductoDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoDto model)
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
        public async Task<IActionResult> Edit(ProductoDto model)
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
    }
}
