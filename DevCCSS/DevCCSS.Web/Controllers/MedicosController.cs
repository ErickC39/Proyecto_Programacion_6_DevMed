using DevCCSS.Web.Contracts;
using DevCCSS.Web.Models;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class MedicosController : Controller
    {
        private readonly MedicoClient _medicoClient;
        private readonly EspecialidadClient _especialidadClient;
        private readonly EmpleadoClient _empleadoClient;

        public MedicosController(
            MedicoClient medicoClient,
            EspecialidadClient especialidadClient,
            EmpleadoClient empleadoClient)
        {
            _medicoClient = medicoClient;
            _especialidadClient = especialidadClient;
            _empleadoClient = empleadoClient;
        }

        // Solo empleados cuyo cargo (el rol de su usuario del sistema) es
        // "Medico" pueden darse de alta como medico -- evita registrar por
        // error a una recepcionista o enfermera, por ejemplo.
        private async Task<List<EmpleadoDto>> CargarEmpleadosMedicos()
        {
            var empleados = await _empleadoClient.ListarAsync();
            return empleados.Where(e => e.Cargo == "Medico").ToList();
        }

        public async Task<IActionResult> Index()
        {
            var medicos = await _medicoClient.ListarAsync();
            return View(medicos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Especialidades = await _especialidadClient.ListarAsync();
            ViewBag.Empleados = await CargarEmpleadosMedicos();

            return View(new MedicoDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            MedicoDto medico,
            int[] dias,
            TimeSpan HoraInicio,
            TimeSpan HoraFin)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Especialidades = await _especialidadClient.ListarAsync();
                ViewBag.Empleados = await CargarEmpleadosMedicos();
                return View(medico);
            }

            try
            {
                var respuesta = await _medicoClient.CrearAsync(medico);

                if (!respuesta.Ok)
                {
                    ViewBag.Error = respuesta.Mensaje;
                    ViewBag.Especialidades = await _especialidadClient.ListarAsync();
                    ViewBag.Empleados = await CargarEmpleadosMedicos();
                    return View(medico);
                }

                var medicos = await _medicoClient.ListarAsync();

                var medicoCreado = medicos
                    .FirstOrDefault(x => x.IdEmpleado == medico.IdEmpleado);

                if (medicoCreado == null)
                {
                    ViewBag.Error = "No se pudo obtener el médico creado.";
                    ViewBag.Especialidades = await _especialidadClient.ListarAsync();
                    ViewBag.Empleados = await CargarEmpleadosMedicos();
                    return View(medico);
                }

                if (dias == null || dias.Length == 0)
                {
                    ViewBag.Error = "Debe seleccionar al menos un día de atención.";
                    ViewBag.Especialidades = await _especialidadClient.ListarAsync();
                    ViewBag.Empleados = await CargarEmpleadosMedicos();
                    return View(medico);
                }

                foreach (var dia in dias)
                {
                    var horario = new HorarioMedicoDto
                    {
                        IdMedico = medicoCreado.IdMedico,
                        DiaSemana = (byte)dia,
                        HoraInicio = HoraInicio,
                        HoraFin = HoraFin
                    };

                    var respuestaHorario =
                        await _medicoClient.AgregarHorarioAsync(horario);

                    if (!respuestaHorario.Ok)
                    {
                        ViewBag.Error = respuestaHorario.Mensaje;
                        ViewBag.Especialidades = await _especialidadClient.ListarAsync();
                        ViewBag.Empleados = await CargarEmpleadosMedicos();
                        return View(medico);
                    }
                }

                TempData["Ok"] = "Medico registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                ViewBag.Especialidades = await _especialidadClient.ListarAsync();
                ViewBag.Empleados = await CargarEmpleadosMedicos();
                return View(medico);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var medico = await _medicoClient.ObtenerPorIdAsync(id);

            if (medico == null)
                return NotFound();

            var horarios =
                await _medicoClient.ListarHorariosAsync(id);

            var citas =
                await _medicoClient.ListarCitasAsignadasAsync(id);

            var modelo = new MedicoDetalleViewModel
            {
                Medico = medico,
                Horarios = horarios,
                Citas = citas
            };

            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var medico = await _medicoClient.ObtenerPorIdAsync(id);
            if (medico == null) return NotFound();

            ViewBag.Especialidades = await _especialidadClient.ListarAsync();
            ViewBag.Horarios = await _medicoClient.ListarHorariosAsync(id);
            return View(medico);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MedicoDto medico)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Especialidades = await _especialidadClient.ListarAsync();
                ViewBag.Horarios = await _medicoClient.ListarHorariosAsync(medico.IdMedico);
                return View(medico);
            }

            var respuesta = await _medicoClient.ActualizarAsync(medico);
            if (!respuesta.Ok)
            {
                ViewBag.Error = respuesta.Mensaje;
                ViewBag.Especialidades = await _especialidadClient.ListarAsync();
                ViewBag.Horarios = await _medicoClient.ListarHorariosAsync(medico.IdMedico);
                return View(medico);
            }

            TempData["Ok"] = respuesta.Mensaje;
            return RedirectToAction(nameof(Details), new { id = medico.IdMedico });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var medico = await _medicoClient.ObtenerPorIdAsync(id);
            if (medico == null) return NotFound();
            return View(medico);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var respuesta = await _medicoClient.EliminarAsync(id);
            TempData[respuesta.Ok ? "Ok" : "Error"] = respuesta.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Horario(int id)
        {
            return View(new HorarioMedicoDto { IdMedico = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Horario(HorarioMedicoDto horario)
        {
            var respuesta = await _medicoClient.AgregarHorarioAsync(horario);

            if (!respuesta.Ok)
            {
                ViewBag.Error = respuesta.Mensaje;
                return View(horario);
            }

            TempData["Ok"] = respuesta.Mensaje;
            return RedirectToAction(nameof(Edit), new { id = horario.IdMedico });
        }
    }
}
