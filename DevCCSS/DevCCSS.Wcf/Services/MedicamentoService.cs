using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class MedicamentoService : IMedicamentoService
    {
        private readonly IConfiguration _config;

        public MedicamentoService(IConfiguration config)
        {
            _config = config;
        }

        public List<MedicamentoDto> Listar()
        {
            var repo = new MedicamentoRepository(_config);
            return repo.Listar();
        }

        public MedicamentoDto? ObtenerPorId(int id)
        {
            var repo = new MedicamentoRepository(_config);
            return repo.ObtenerPorId(id);
        }

        public RespuestaCrud Crear(MedicamentoDto medicamento)
        {
            var repo = new MedicamentoRepository(_config);
            try
            {
                return repo.Crear(medicamento);
            }
            catch (Exception ex)
            {
                
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }

        public RespuestaCrud Actualizar(MedicamentoDto medicamento)
        {
            var repo = new MedicamentoRepository(_config);
            try
            {
                return repo.Actualizar(medicamento);
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }

        public RespuestaCrud Eliminar(int id)
        {
            var repo = new MedicamentoRepository(_config);
            try
            {
                return repo.Eliminar(id);
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }
    }
}