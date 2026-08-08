using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class EspecialidadService : IEspecialidadService
    {
        private readonly IConfiguration _config;

        public EspecialidadService(IConfiguration config)
        {
            _config = config;
        }

        public List<EspecialidadDto> Listar()
        {
            return new EspecialidadRepository(_config).Listar();
        }
    }
}
