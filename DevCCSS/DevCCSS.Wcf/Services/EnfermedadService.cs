using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class EnfermedadService : IEnfermedadService
    {
        private readonly IConfiguration _config;
        public EnfermedadService(IConfiguration config) { _config = config; }

        public List<EnfermedadDto> Listar() => new EnfermedadRepository(_config).Listar();
        public EnfermedadDto? ObtenerPorId(int id) => new EnfermedadRepository(_config).ObtenerPorId(id);

        public RespuestaCrud Crear(EnfermedadDto enfermedad)
        {
            try { return new EnfermedadRepository(_config).Crear(enfermedad); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
        public RespuestaCrud Actualizar(EnfermedadDto enfermedad)
        {
            try { return new EnfermedadRepository(_config).Actualizar(enfermedad); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
        public RespuestaCrud Eliminar(int id)
        {
            try { return new EnfermedadRepository(_config).Eliminar(id); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }

        public List<TratamientoEnfermedadDto> ListarMedicamentos(int idEnfermedad)
            => new TratamientoEnfermedadRepository(_config).ListarPorEnfermedad(idEnfermedad);

        public RespuestaCrud AsignarMedicamento(int idEnfermedad, int idMedicamento, string? observacion)
        {
            try { return new TratamientoEnfermedadRepository(_config).Asignar(idEnfermedad, idMedicamento, observacion); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }

        public RespuestaCrud QuitarMedicamento(int idEnfermedad, int idMedicamento)
        {
            try { return new TratamientoEnfermedadRepository(_config).Quitar(idEnfermedad, idMedicamento); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
    }
}
