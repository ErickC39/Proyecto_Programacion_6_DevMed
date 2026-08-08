using DevCCSS.Web.Common;
using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador,Medico,Enfermeria,Recepcionista")]
    [Modulo("Citas")]
    public class CitasController : Controller
    {
        private readonly CitaClient _citas;
        private readonly EmpleadoClient _empleados;
        private readonly MedicoClient _medicos;
        private readonly EspecialidadClient _especialidades;
        private readonly ExamenMedicoClient _examenes;

        public CitasController(CitaClient citas, EmpleadoClient empleados, MedicoClient medicos, EspecialidadClient especialidades, ExamenMedicoClient examenes)
        {
            _citas = citas;
            _empleados = empleados;
            _medicos = medicos;
            _especialidades = especialidades;
            _examenes = examenes;
        }

        // Carga los medicos (activos) junto con su especialidad -- vía la relacion
        // Medicos->Especialidades -- para que el formulario pueda filtrar segun la
        // necesidad del paciente en vez de depender del texto libre Empleados.Especialidad.
        private async Task CargarMedicos()
        {
            ViewBag.Medicos = (await _medicos.ListarAsync()).Where(m => m.Activo).ToList();
            ViewBag.Especialidades = await _especialidades.ListarAsync();
        }

        public async Task<IActionResult> Agendar()
        {
            await CargarMedicos();
            ViewBag.Pacientes = await _examenes.ListarPacientesAsync();
            return View(new AgendarCitaDto { FechaHoraCita = DateTime.Now.AddHours(1) });
        }
        public async Task<IActionResult> Index()
        {
            var lista = await _citas.ListarAsync();
            return View(lista);
        }

        public async Task<IActionResult> Details(int id)
        {
            var cita = await _citas.ObtenerPorIdAsync(id);
            if (cita is null) return NotFound();

            if (cita.EstadoCita == "En espera" && cita.TiempoEsperaMinutos > 60)
            {
              
                ViewBag.AvisoDemora = cita.CitaPreviaEsControl
                    ? "El tiempo de espera supera la hora porque la cita anterior con este médico " +
                      "es una cita de control/revisión, por lo que puede tomar más tiempo de lo habitual."
                    : "El tiempo de espera ya supera el máximo recomendado de 1 hora. " +
                      "Esto se debe a que la cita anterior con este médico está tomando más tiempo del previsto.";
            }
            return View(cita);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Agendar(AgendarCitaDto model)
        {
            if (model.FechaHoraCita <= DateTime.Now)
            {
                ModelState.AddModelError(nameof(model.FechaHoraCita), "La fecha y hora de la cita debe ser futura.");
            }

            if (!ModelState.IsValid)
            {
                await CargarMedicos();
                ViewBag.Pacientes = await _examenes.ListarPacientesAsync();
                return View(model);
            }

            var r = await _citas.AgendarAsync(model);

            if (!r.Ok)
            {
                if (r.CodigoDisponibilidad == 1)
                    ModelState.AddModelError("PacienteIdentificacion", r.Mensaje);
                else if (r.CodigoDisponibilidad == 2)
                    ModelState.AddModelError("FechaHoraCita", r.Mensaje + " Por favor seleccione una fecha diferente.");
                else if (r.CodigoDisponibilidad == 3)
                {
                    ModelState.AddModelError("FechaHoraCita", r.Mensaje);
                    ViewBag.HoraSugerida = r.HoraSugerida;
                }
                else
                    ModelState.AddModelError("", r.Mensaje);
                await CargarMedicos();
                ViewBag.Pacientes = await _examenes.ListarPacientesAsync();
                return View(model);
            }

            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> AgendarEmergencia(int? idEmpleado)
        {
            await CargarMedicos();
            ViewBag.EmpleadosVinculados = (await _empleados.ListarAsync()).Where(e => e.IdPacienteVinculado.HasValue).ToList();
            return View(new AgendarEmergenciaDto { IdEmpleado = idEmpleado ?? 0, FechaHoraCita = DateTime.Now.AddMinutes(15) });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AgendarEmergencia(AgendarEmergenciaDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarMedicos();
                ViewBag.EmpleadosVinculados = (await _empleados.ListarAsync()).Where(e => e.IdPacienteVinculado.HasValue).ToList();
                return View(model);
            }
            var r = await _citas.AgendarEmergenciaAsync(model);
            if (!r.Ok)
            {
                ModelState.AddModelError("", r.Mensaje);
                await CargarMedicos();
                ViewBag.EmpleadosVinculados = (await _empleados.ListarAsync()).Where(e => e.IdPacienteVinculado.HasValue).ToList();
                return View(model);
            }
            TempData["Ok"] = r.CitasReagendadas > 0
                ? $"{r.Mensaje} Se reagendaron {r.CitasReagendadas} cita(s) que chocaban con este horario."
                : r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegistrarLlegada(int id)
        {
            var r = await _citas.RegistrarLlegadaAsync(id);
            TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IniciarAtencion(int id)
        {
            var r = await _citas.IniciarAtencionAsync(id);
            TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Finalizar(int id)
        {
            var cita = await _citas.ObtenerPorIdAsync(id);
            if (cita is null) return NotFound();
            if (cita.EstadoCita != "En Progreso")
            {
                TempData["Error"] = "Solo se puede finalizar una cita en estado 'En Progreso'.";
                return RedirectToAction(nameof(Details), new { id });
            }
            return View(new FinalizarCitaDto { IdCita = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Finalizar(FinalizarCitaDto model)
        {
            if (!ModelState.IsValid) return View(model);

            var r = await _citas.FinalizarAsync(model);

            if (!r.Ok)
            {
                ModelState.AddModelError("", r.Mensaje);
                return View(model);
            }

            TempData["Ok"] = r.IdCitaControl > 0
                ? r.Mensaje + $" (ID cita de control: {r.IdCitaControl})"
                : r.Mensaje;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(int id)
        {
            var r = await _citas.CancelarAsync(id);

            TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var cita = await _citas.ObtenerPorIdAsync(id);
            if (cita is null) return NotFound();
            return View(cita);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var r = await _citas.EliminarAsync(id);
                TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
            }
            catch (Exception ex)
            {
                TempData["Error"] = "No se pudo eliminar la cita: " + ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}