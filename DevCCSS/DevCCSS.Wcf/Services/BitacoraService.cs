using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class BitacoraService : IBitacoraService
    {
        private readonly IConfiguration _config;
        public BitacoraService(IConfiguration config) { _config = config; }

        public List<BitacoraAuditoriaDto> ListarAuditoria(int top)
            => new BitacoraRepository(_config).ListarAuditoria(top <= 0 ? 100 : top);

        public List<BitacoraErrorDto> ListarErrores(int top)
            => new BitacoraRepository(_config).ListarErrores(top <= 0 ? 200 : top);

        public List<BitacoraNotificacionDto> ListarNotificaciones(int top)
            => new BitacoraRepository(_config).ListarNotificaciones(top <= 0 ? 200 : top);
    }
}
