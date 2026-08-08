using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class ExamenMedicoService : IExamenMedicoService
    {
        private readonly IConfiguration _config;

        public ExamenMedicoService(IConfiguration config)
        {
            _config = config;
        }

        public List<ExamenMedicoDto> Listar()
        {
            var repo = new ExamenMedicoRepository(_config);
            return repo.Listar();
        }

        public List<ExamenMedicoDto> ListarPorPaciente(int idPaciente)
        {
            var repo = new ExamenMedicoRepository(_config);
            return repo.ListarPorPaciente(idPaciente);
        }

        public ExamenMedicoDto? ObtenerPorId(int id)
        {
            var repo = new ExamenMedicoRepository(_config);
            return repo.ObtenerPorId(id);
        }

        public List<TipoExamenDto> ListarTiposExamen()
        {
            var repo = new ExamenMedicoRepository(_config);
            return repo.ListarTiposExamen();
        }

        public List<EstadoExamenDto> ListarEstadosExamen()
        {
            var repo = new ExamenMedicoRepository(_config);
            return repo.ListarEstadosExamen();
        }

        public List<PacienteExamenDto> ListarPacientes()
        {
            var repo = new ExamenMedicoRepository(_config);
            return repo.ListarPacientes();
        }

        public List<MedicoExamenDto> ListarMedicos()
        {
            var repo = new ExamenMedicoRepository(_config);
            return repo.ListarMedicos();
        }

        public RespuestaCrud Crear(ExamenMedicoDto examen)
        {
            var repo = new ExamenMedicoRepository(_config);
            try
            {
                return repo.Crear(examen);
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }

        public RespuestaCrud Actualizar(ExamenMedicoDto examen)
        {
            var repo = new ExamenMedicoRepository(_config);
            try
            {
                return repo.Actualizar(examen);
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }

        public RespuestaCrud Eliminar(int id)
        {
            var repo = new ExamenMedicoRepository(_config);
            try
            {
                return repo.Eliminar(id);
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }

        public RespuestaCrud CrearTipoExamen(TipoExamenDto tipo)
        {
            var repo = new ExamenMedicoRepository(_config);
            try
            {
                return repo.CrearTipoExamen(tipo);
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }
    }
}
