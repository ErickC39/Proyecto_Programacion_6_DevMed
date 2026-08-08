using DevCCSS.Web.Common;
using DevCCSS.Web.Contracts;
using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize(Roles = "Administrador,Medico,Enfermeria")]
    [Modulo("ExamenesMedicos")]
    public class ExamenesMedicosController : Controller
    {
        private readonly ExamenMedicoClient _examenes;

        public ExamenesMedicosController(ExamenMedicoClient examenes)
        {
            _examenes = examenes;
        }

        private async Task CargarCatalogos()
        {
            ViewBag.TiposExamen = await _examenes.ListarTiposExamenAsync();
            ViewBag.EstadosExamen = await _examenes.ListarEstadosExamenAsync();
            ViewBag.Pacientes = await _examenes.ListarPacientesAsync();
            ViewBag.Medicos = await _examenes.ListarMedicosAsync();
        }

        // GET: /ExamenesMedicos
        public async Task<IActionResult> Index()
        {
            var lista = await _examenes.ListarAsync();
            return View(lista);
        }

        // GET: /ExamenesMedicos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var examen = await _examenes.ObtenerPorIdAsync(id);
            if (examen is null) return NotFound();
            return View(examen);
        }

        // GET: /ExamenesMedicos/Create
        [Authorize(Roles = "Administrador,Medico")]
        public async Task<IActionResult> Create(int? idPaciente)
        {
            await CargarCatalogos();
            var estados = (List<EstadoExamenDto>)ViewBag.EstadosExamen;
            return View(new ExamenMedicoDto
            {
                IdPaciente = idPaciente ?? 0,
                FechaSolicitud = DateTime.Now,
                IdEstadoExamen = estados
                    .FirstOrDefault(x => x.Descripcion == "Solicitado")
                    ?.IdEstadoExamen ?? 0
            });
        }

        // POST: /ExamenesMedicos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Medico")]
        public async Task<IActionResult> Create(ExamenMedicoDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogos();
                return View(model);
            }

            var respuesta = await _examenes.CrearAsync(model);
            if (!respuesta.Ok)
            {
                ModelState.AddModelError("", respuesta.Mensaje);
                await CargarCatalogos();
                return View(model);
            }

            TempData["Ok"] = respuesta.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        // GET: /ExamenesMedicos/Edit/5
        [Authorize(Roles = "Administrador,Medico")]
        public async Task<IActionResult> Edit(int id)
        {
            var examen = await _examenes.ObtenerPorIdAsync(id);
            if (examen is null) return NotFound();
            await CargarCatalogos();
            return View(examen);
        }

        // POST: /ExamenesMedicos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Medico")]
        public async Task<IActionResult> Edit(ExamenMedicoDto model)
        {
            if (!ModelState.IsValid)
            {
                await CargarCatalogos();
                return View(model);
            }

            var respuesta = await _examenes.ActualizarAsync(model);
            if (!respuesta.Ok)
            {
                ModelState.AddModelError("", respuesta.Mensaje);
                await CargarCatalogos();
                return View(model);
            }

            TempData["Ok"] = respuesta.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        // GET: /ExamenesMedicos/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int id)
        {
            var examen = await _examenes.ObtenerPorIdAsync(id);
            if (examen is null) return NotFound();
            return View(examen);
        }

        // POST: /ExamenesMedicos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var respuesta = await _examenes.EliminarAsync(id);
            TempData[respuesta.Ok ? "Ok" : "Error"] = respuesta.Mensaje;
            return RedirectToAction(nameof(Index));
        }

        // GET: /ExamenesMedicos/TiposExamen
        public async Task<IActionResult> TiposExamen()
        {
            var lista = await _examenes.ListarTiposExamenAsync();
            return View(lista);
        }

        // GET: /ExamenesMedicos/CrearTipoExamen
        [Authorize(Roles = "Administrador,Medico")]
        public IActionResult CrearTipoExamen()
        {
            return View(new TipoExamenDto());
        }

        // POST: /ExamenesMedicos/CrearTipoExamen
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Medico")]
        public async Task<IActionResult> CrearTipoExamen(TipoExamenDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var respuesta = await _examenes.CrearTipoExamenAsync(model);
            if (!respuesta.Ok)
            {
                ModelState.AddModelError("", respuesta.Mensaje);
                return View(model);
            }

            TempData["Ok"] = respuesta.Mensaje;
            return RedirectToAction(nameof(TiposExamen));
        }
    }
}
