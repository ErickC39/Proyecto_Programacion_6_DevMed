using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuariosController : Controller
    {
        private readonly UsuarioClient _usuarios;
        public UsuariosController(UsuarioClient usuarios) { _usuarios = usuarios; }

        private async Task CargarRoles()
        {
            ViewBag.Roles = await _usuarios.ListarRolesAsync();
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _usuarios.ListarAsync();
            return View(lista);
        }

        public async Task<IActionResult> Details(int id)
        {
            var u = await _usuarios.ObtenerPorIdAsync(id);
            if (u is null) return NotFound();
            return View(u);
        }

        public async Task<IActionResult> Create()
        {
            await CargarRoles();
            return View(new UsuarioDto { Activo = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioDto model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
                ModelState.AddModelError("Password", "La contrasenia es obligatoria al crear.");
            if (!ModelState.IsValid) { await CargarRoles(); return View(model); }

            var r = await _usuarios.CrearAsync(model);
            if (!r.Ok) { ModelState.AddModelError("", r.Mensaje); await CargarRoles(); return View(model); }
            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var u = await _usuarios.ObtenerPorIdAsync(id);
            if (u is null) return NotFound();
            await CargarRoles();
            return View(u);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioDto model)
        {
            if (!ModelState.IsValid) { await CargarRoles(); return View(model); }
            var r = await _usuarios.ActualizarAsync(model);
            if (!r.Ok) { ModelState.AddModelError("", r.Mensaje); await CargarRoles(); return View(model); }
            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var u = await _usuarios.ObtenerPorIdAsync(id);
            if (u is null) return NotFound();
            return View(u);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var r = await _usuarios.EliminarAsync(id);
            TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }
    }
}
