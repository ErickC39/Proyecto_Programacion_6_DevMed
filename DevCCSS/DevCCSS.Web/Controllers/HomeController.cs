using DevCCSS.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevCCSS.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly PacienteClient _pacientes;
        private readonly InventarioClient _inventario;
        private readonly MedicamentoClient _medicamentos;

        public HomeController(PacienteClient pacientes, InventarioClient inventario, MedicamentoClient medicamentos)
        {
            _pacientes = pacientes;
            _inventario = inventario;
            _medicamentos = medicamentos;
        }

        public async Task<IActionResult> Index()
        {
           
            ViewBag.TotalPacientes = await ContarAsync(async () => (await _pacientes.ListarAsync()).Count);
            ViewBag.TotalInventario = await ContarAsync(async () => (await _inventario.ListarAsync()).Count);
            ViewBag.TotalMedicamentos = await ContarAsync(async () => (await _medicamentos.ListarAsync()).Count);

            return View();
        }

        public IActionResult AcercaDe()
        {
            return View();
        }

        public IActionResult Ayuda()
        {
            return View();
        }

        public IActionResult Terminos()
        {
            return View();
        }

        // Helper: si el WCF no responde, devolvemos 0 y el dashboard no se cae.
        private static async Task<int> ContarAsync(Func<Task<int>> contar)
        {
            try { return await contar(); }
            catch { return 0; }
        }
    }
}
