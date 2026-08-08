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

        public MedicosController(
            MedicoClient medicoClient,
            EspecialidadClient especialidadClient)
        {
            _medicoClient = medicoClient;
            _especialidadClient = especialidadClient;
        }

        public async Task<IActionResult> Index()
        {
            var medicos = await _medicoClient.ListarAsync();
            return View(medicos);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var especialidades = await _especialidadClient.ListarAsync();
            ViewBag.Especialidades = especialidades;

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
            try
            {
                var respuesta = await _medicoClient.CrearAsync(medico);

                if (!respuesta.Ok)
                {
                    ViewBag.Error = respuesta.Mensaje;
                    ViewBag.Especialidades = await _especialidadClient.ListarAsync();
                    return View(medico);
                }

                var medicos = await _medicoClient.ListarAsync();

                var medicoCreado = medicos
                    .FirstOrDefault(x => x.IdEmpleado == medico.IdEmpleado);

                if (medicoCreado == null)
                {
                    ViewBag.Error = "No se pudo obtener el médico creado.";
                    ViewBag.Especialidades = await _especialidadClient.ListarAsync();
                    return View(medico);
                }

                if (dias == null || dias.Length == 0)
                {
                    ViewBag.Error = "Debe seleccionar al menos un día de atención.";
                    ViewBag.Especialidades = await _especialidadClient.ListarAsync();
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
                return View(medico);
            }
        }

        // BUSCAR EMPLEADO POR IDENTIFICACION
        [HttpGet]
        public async Task<IActionResult> BuscarEmpleado(string identificacion)
        {
            var empleado =
                await _medicoClient.BuscarEmpleadoAsync(identificacion);

            if (empleado == null)
            {
                return NotFound(new
                {
                    mensaje = "Empleado no encontrado"
                });
            }

            return Json(new
            {
                idEmpleado = empleado.IdEmpleado,
                identificacion = empleado.Identificacion,
                nombre = empleado.Nombre,
                apellidos = empleado.Apellidos
            });
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
            return RedirectToAction(nameof(Details), new { id = horario.IdMedico });
        }
    }
}
