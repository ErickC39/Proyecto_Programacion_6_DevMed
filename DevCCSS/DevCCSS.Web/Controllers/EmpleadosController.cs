using DevCCSS.Web.Common;
using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    [Modulo("Empleados")]
    public class EmpleadosController : Controller
    {
        private readonly EmpleadoClient _servicio;
        private readonly UsuarioClient _usuarios;

        public EmpleadosController(EmpleadoClient servicio, UsuarioClient usuarios)
        {
            _servicio = servicio;
            _usuarios = usuarios;
        }

        private async Task CargarListas()
        {
            ViewBag.Usuarios = await _usuarios.ListarAsync();
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
            return View(new EmpleadoDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmpleadoDto model)
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
        public async Task<IActionResult> Edit(EmpleadoDto model)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CambiarEstado(int id, bool activo)
        {
            var r = await _servicio.CambiarEstadoAsync(id, activo);
            TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
