using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class NacimientoService : INacimientoService
    {
        private readonly IConfiguration _config;
        public NacimientoService(IConfiguration config) { _config = config; }

        public List<NacimientoDto> Listar() => new NacimientoRepository(_config).Listar();

        public RespuestaCrud Registrar(NacimientoDto nacimiento)
        {
            try { return new NacimientoRepository(_config).Registrar(nacimiento); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }

        public RespuestaCrud RegistrarCompleto(RegistrarNacimientoDto nacimiento)
        {
            try { return new NacimientoRepository(_config).RegistrarCompleto(nacimiento); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }

        public NacimientoDetalleDto? ObtenerDetalle(int idPaciente)
            => new NacimientoRepository(_config).ObtenerDetalle(idPaciente);

        public RespuestaCrud Actualizar(RegistrarNacimientoDto nacimiento)
        {
            try { return new NacimientoRepository(_config).Actualizar(nacimiento); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }

        public RespuestaCrud Eliminar(int idPaciente)
        {
            try { return new NacimientoRepository(_config).Eliminar(idPaciente); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
    }
}