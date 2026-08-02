using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador,Medico,Enfermeria")]
    public class HabitacionesController : Controller
    {
        private readonly HabitacionClient _habitaciones;

        public HabitacionesController(HabitacionClient habitaciones)
        {
            _habitaciones = habitaciones;
        }

        private async Task CargarCatalogos()
        {
            ViewBag.TiposHabitacion = await _habitaciones.ListarTiposHabitacionAsync();
        }

        private async Task CargarCatalogosAsignacion()
        {
            ViewBag.Pacientes = await _habitaciones.ListarPacientesAsync();
            ViewBag.Empleados = await _habitaciones.ListarEmpleadosAsync();
        }

        // GET: /Habitaciones
        public async Task<IActionResult> Index()
        {
            var lista = await _habitaciones.ListarAsync();
            return View(lista);
        }

        // GET: /Habitaciones/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var habitacion = await _habitaciones.ObtenerPorIdAsync(id);
            if (habitacion is null) return NotFound();
            return View(habitacion);
        }

        // GET: /Habitaciones/Create
        [Authorize(Roles = "Administrador,Enfermeria")]
        public async Task<IActionResult> Create()
        {
            await CargarCatalogos();
            return View(new HabitacionDto());
        }

        // POST: /Habitaciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Enfermeria")]
        public async Task<IActionResult> Create(HabitacionDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogos();
                return View(model);
            }

            var respuesta = await _habitaciones.CrearAsync(model);
            if (!respuesta.Ok)
            {
                ModelState.AddModelError("", respuesta.Mensaje);
                await CargarCatalogos();
                return View(model);
            }

            TempData["Ok"] = respuesta.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Habitaciones/Edit/5
        [Authorize(Roles = "Administrador,Enfermeria")]
        public async Task<IActionResult> Edit(int id)
        {
            var habitacion = await _habitaciones.ObtenerPorIdAsync(id);
            if (habitacion is null) return NotFound();
            await CargarCatalogos();
            return View(habitacion);
        }

        // POST: /Habitaciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Enfermeria")]
        public async Task<IActionResult> Edit(HabitacionDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogos();
                return View(model);
            }

            var respuesta = await _habitaciones.ActualizarAsync(model);
            if (!respuesta.Ok)
            {
                ModelState.AddModelError("", respuesta.Mensaje);
                await CargarCatalogos();
                return View(model);
            }

            TempData["Ok"] = respuesta.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Habitaciones/Asignar/5
        [Authorize(Roles = "Administrador,Enfermeria")]
        public async Task<IActionResult> Asignar(int id)
        {
            var habitacion = await _habitaciones.ObtenerPorIdAsync(id);
            if (habitacion is null) return NotFound();
            if (habitacion.EstadoHabitacion == "Ocupada")
            {
                TempData["Error"] = "La habitacion ya esta ocupada. Debe liberarla antes de asignarla nuevamente.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await CargarCatalogosAsignacion();
            return View(new AsignarHabitacionDto { IdHabitacion = id, FechaIngreso = DateTime.Now });
        }

        // POST: /Habitaciones/Asignar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Enfermeria")]
        public async Task<IActionResult> Asignar(AsignarHabitacionDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogosAsignacion();
                return View(model);
            }

            var respuesta = await _habitaciones.AsignarAsync(model);
            if (!respuesta.Ok)
            {
                ModelState.AddModelError("", respuesta.Mensaje);
                await CargarCatalogosAsignacion();
                return View(model);
            }

            TempData["Ok"] = respuesta.Mensaje;
            return RedirectToAction(nameof(Details), new { id = model.IdHabitacion });
        }

        // GET: /Habitaciones/Liberar/5
        [Authorize(Roles = "Administrador,Enfermeria")]
        public async Task<IActionResult> Liberar(int id)
        {
            var habitacion = await _habitaciones.ObtenerPorIdAsync(id);
            if (habitacion is null) return NotFound();
            if (habitacion.EstadoHabitacion != "Ocupada")
            {
                TempData["Error"] = "La habitacion no esta ocupada actualmente.";
                return RedirectToAction(nameof(Details), new { id });
            }

            await CargarCatalogosAsignacion();
            return View(habitacion);
        }

        // POST: /Habitaciones/Liberar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Enfermeria")]
        public async Task<IActionResult> Liberar(LiberarHabitacionDto model)
        {
            var respuesta = await _habitaciones.LiberarAsync(model);
            TempData[respuesta.Ok ? "Ok" : "Error"] = respuesta.Mensaje;
            return RedirectToAction(nameof(Details), new { id = model.IdHabitacion });
        }

        // GET: /Habitaciones/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var habitacion = await _habitaciones.ObtenerPorIdAsync(id);
            if (habitacion is null) return NotFound();
            return View(habitacion);
        }

        // POST: /Habitaciones/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var respuesta = await _habitaciones.EliminarAsync(id);
            TempData[respuesta.Ok ? "Ok" : "Error"] = respuesta.Mensaje;
            return RedirectToAction(nameof(Index));
        }
    }
}
