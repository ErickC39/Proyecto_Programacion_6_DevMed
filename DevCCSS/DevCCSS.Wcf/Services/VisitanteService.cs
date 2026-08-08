using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class VisitanteService : IVisitanteService
    {
        private readonly IConfiguration _config;
        public VisitanteService(IConfiguration config) { _config = config; }

        public List<VisitanteDto> Listar() => new VisitanteRepository(_config).Listar();
        public VisitanteDto? ObtenerPorId(int id) => new VisitanteRepository(_config).ObtenerPorId(id);

        public RespuestaCrud Crear(VisitanteDto visitante)
        {
            try { return new VisitanteRepository(_config).Crear(visitante); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
        public RespuestaCrud Actualizar(VisitanteDto visitante)
        {
            try { return new VisitanteRepository(_config).Actualizar(visitante); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
        public RespuestaCrud Eliminar(int id)
        {
            try { return new VisitanteRepository(_config).Eliminar(id); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
        public RespuestaCrud RegistrarSalida(int idVisita, DateTime? fechaHoraSalida)
        {
            try { return new VisitanteRepository(_config).RegistrarSalida(idVisita, fechaHoraSalida); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
    }
}
