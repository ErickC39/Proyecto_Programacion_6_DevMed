using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class PermisoService : IPermisoService
    {
        private readonly IConfiguration _config;
        public PermisoService(IConfiguration config) { _config = config; }

        public List<RolPermisoDto> Listar() => new RolPermisoRepository(_config).Listar();
    }
}
