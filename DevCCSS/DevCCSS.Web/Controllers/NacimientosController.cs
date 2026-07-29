using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador,Medico,Enfermeria")]
    public class NacimientosController : Controller
    {
        // Tamizajes neonatales estandar que se piden siempre (Costa Rica: CCSS).
        private static readonly string[] TiposTamizaje =
        {
            "Metabolico neonatal", "Auditivo", "Cardiopatias congenitas criticas", "Visual / reflejo rojo"
        };

        private readonly NacimientoClient _nacimientos;
        private readonly PacienteClient _pacientes;

        public NacimientosController(NacimientoClient nacimientos, PacienteClient pacientes)
        {
            _nacimientos = nacimientos;
            _pacientes = pacientes;
        }

        private async Task CargarListas()
        {
            ViewBag.TiposSangre = await _pacientes.ListarTiposSangreAsync();
            ViewBag.SexosBiologicos = await _pacientes.ListarSexosBiologicosAsync();
            var todos = await _pacientes.ListarAsync();
            ViewBag.Madres = todos.Where(p => p.SexoBiologico == "Femenino").ToList();
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _nacimientos.ListarAsync();
            return View(lista);
        }

        public async Task<IActionResult> Details(int id)
        {
            var detalle = await _nacimientos.ObtenerDetalleAsync(id);
            if (detalle is null) return NotFound();
            return View(detalle);
        }

        public async Task<IActionResult> Registrar()
        {
            await CargarListas();
            var model = new RegistrarNacimientoDto
            {
                FechaNacimiento = DateTime.Today,
                Apgar = new List<ApgarDto> { new() { Minuto = 1 }, new() { Minuto = 5 } },
                Tamizajes = TiposTamizaje.Select(t => new TamizajeDto { TipoTamizaje = t }).ToList()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registrar(RegistrarNacimientoDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarListas();
                return View(model);
            }

            // El formulario captura la estatura en cm (uso clinico habitual en recien nacidos);
            // se guarda en metros para ser consistente con el resto del modulo de Pacientes.
            if (model.Estatura.HasValue) model.Estatura = model.Estatura.Value / 100m;

            var r = await _nacimientos.RegistrarCompletoAsync(model);
            if (!r.Ok)
            {
                ModelState.AddModelError("", r.Mensaje);
                await CargarListas();
                return View(model);
            }
            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var detalle = await _nacimientos.ObtenerDetalleAsync(id);
            if (detalle?.Datos is null) return NotFound();

            var model = new RegistrarNacimientoDto
            {
                IdPaciente = detalle.Datos.IdPaciente,
                Nombre = detalle.Datos.Nombre,
                Apellidos = detalle.Datos.Apellidos,
                FechaNacimiento = detalle.Datos.FechaNacimiento,
                IdSexoBiologico = detalle.Datos.IdSexoBiologico,
                IdTipoSangre = detalle.Datos.IdTipoSangre,
                Peso = detalle.Datos.Peso,
                Estatura = detalle.Datos.Estatura.HasValue ? detalle.Datos.Estatura.Value * 100m : null, // m -> cm para editar
                IdMadre = detalle.Datos.IdMadre,
                AntecedentesMedicos = detalle.Datos.AntecedentesMedicos,
                Alergias = detalle.Datos.Alergias,
                PerimetroCefalico = detalle.PerimetroCefalico,
                PerimetroToracico = detalle.PerimetroToracico,
                Temperatura = detalle.Temperatura,
                FrecuenciaCardiaca = detalle.FrecuenciaCardiaca,
                FrecuenciaRespiratoria = detalle.FrecuenciaRespiratoria,
                Reflejos = detalle.Reflejos,
                ColoracionPiel = detalle.ColoracionPiel,
                ObservacionesGenerales = detalle.ObservacionesGenerales,
                Apgar = new List<ApgarDto>
                {
                    detalle.Apgar.FirstOrDefault(a => a.Minuto == 1) ?? new ApgarDto { Minuto = 1 },
                    detalle.Apgar.FirstOrDefault(a => a.Minuto == 5) ?? new ApgarDto { Minuto = 5 }
                },
                Tamizajes = TiposTamizaje.Select(t =>
                    detalle.Tamizajes.FirstOrDefault(x => x.TipoTamizaje == t) ?? new TamizajeDto { TipoTamizaje = t }
                ).ToList()
            };

            ViewBag.Identificacion = detalle.Datos.Identificacion;
            await CargarListas();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RegistrarNacimientoDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarListas();
                return View(model);
            }

            if (model.Estatura.HasValue) model.Estatura = model.Estatura.Value / 100m;

            var r = await _nacimientos.ActualizarAsync(model);
            if (!r.Ok)
            {
                ModelState.AddModelError("", r.Mensaje);
                await CargarListas();
                return View(model);
            }
            TempData["Ok"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            var detalle = await _nacimientos.ObtenerDetalleAsync(id);
            if (detalle?.Datos is null) return NotFound();
            return View(detalle.Datos);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var r = await _nacimientos.EliminarAsync(id);
            TempData[r.Ok ? "Ok" : "Error"] = r.Mensaje;
            return RedirectToAction(nameof(Index));
        }
    }
}
