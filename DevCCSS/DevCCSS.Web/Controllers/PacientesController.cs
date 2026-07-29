using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador,Medico,Enfermeria,Recepcionista")]
    public class PacientesController : Controller
    {
        private readonly PacienteClient _pacientes;

        public PacientesController(PacienteClient pacientes)
        {
            _pacientes = pacientes;
        }

        // Carga los catalogos (sangre, sexo, identidad) para los dropdowns
        private async Task CargarCatalogos()
        {
            ViewBag.TiposSangre = await _pacientes.ListarTiposSangreAsync();
            ViewBag.SexosBiologicos = await _pacientes.ListarSexosBiologicosAsync();
            ViewBag.IdentidadesGenero = await _pacientes.ListarIdentidadesGeneroAsync();
        }

        // GET: /Pacientes
        public async Task<IActionResult> Index()
        {
            var lista = await _pacientes.ListarAsync();
            return View(lista);
        }

        // GET: /Pacientes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var p = await _pacientes.ObtenerPorIdAsync(id);
            if (p is null) return NotFound();
            return View(p);
        }

        // GET: /Pacientes/Create
        public async Task<IActionResult> Create()
        {
            await CargarCatalogos();
            return View(new PacienteDto { FechaNacimiento = DateTime.Today });
        }

        // POST: /Pacientes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogos();
                return View(model);
            }

            var r = await _pacientes.CrearAsync(model);
            if (!r.Ok)
            {
                ModelState.AddModelError("", r.Mensaje);
                await CargarCatalogos();
                return View(model);
            }
            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Pacientes/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var p = await _pacientes.ObtenerPorIdAsync(id);
            if (p is null) return NotFound();
            await CargarCatalogos();
            return View(p);
        }

        // POST: /Pacientes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PacienteDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogos();
                return View(model);
            }

            var r = await _pacientes.ActualizarAsync(model);
            if (!r.Ok)
            {
                ModelState.AddModelError("", r.Mensaje);
                await CargarCatalogos();
                return View(model);
            }
            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Pacientes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _pacientes.ObtenerPorIdAsync(id);
            if (p is null) return NotFound();
            return View(p);
        }

        // POST: /Pacientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var r = await _pacientes.EliminarAsync(id);
            TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        // GET: /Pacientes/Expediente/5
        public async Task<IActionResult> Expediente(int id)
        {
            var p = await _pacientes.ObtenerPorIdAsync(id);
            if (p is null) return NotFound();

            var exp = new ExpedienteDto
            {
                IdPaciente = p.IdPaciente,
                AntecedentesMedicos = p.AntecedentesMedicos,
                IdTipoSangre = p.IdTipoSangre,
                Peso = p.Peso,
                Estatura = p.Estatura,
                Alergias = p.Alergias
            };
            ViewData["NombrePaciente"] = $"{p.Nombre} {p.Apellidos}";
            await CargarCatalogos();
            return View(exp);
        }

        // POST: /Pacientes/Expediente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Expediente(ExpedienteDto model)
        {
            var r = await _pacientes.GuardarExpedienteAsync(model);
            if (!r.Ok)
            {
                ModelState.AddModelError("", r.Mensaje);
                ViewData["NombrePaciente"] = "Paciente #" + model.IdPaciente;
                await CargarCatalogos();
                return View(model);
            }
            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }
    }
}