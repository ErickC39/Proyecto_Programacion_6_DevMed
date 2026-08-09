using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class PacienteService : IPacienteService
    {
        private readonly IConfiguration _config;

        public PacienteService(IConfiguration config)
        {
            _config = config;
        }

        public List<PacienteDto> Listar()
        {
            var repo = new PacienteRepository(_config);
            return repo.Listar();
        }

        public PacienteDto? ObtenerPorId(int id)
        {
            var repo = new PacienteRepository(_config);
            return repo.ObtenerPorId(id);
        }

        public RespuestaCrud Crear(PacienteDto paciente)
        {
            var repo = new PacienteRepository(_config);
            try
            {
                return repo.Crear(paciente);
            }
            catch (Exception ex)
            {
                // El SP ya hace ROLLBACK y guarda en Bitacora_Errores.
                // Aqui solo devolvemos el mensaje al cliente.
                return new RespuestaCrud { Ok = false, Mensaje = "No se pudo crear: " + ex.Message };
            }
        }

        public RespuestaCrud Actualizar(PacienteDto paciente)
        {
            var repo = new PacienteRepository(_config);
            try
            {
                return repo.Actualizar(paciente);
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = "No se pudo actualizar: " + ex.Message };
            }
        }

        public RespuestaCrud Eliminar(int id)
        {
            var repo = new PacienteRepository(_config);
            try
            {
                return repo.Eliminar(id);
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = "No se pudo eliminar: " + ex.Message };
            }
        }

        public RespuestaCrud GuardarExpediente(ExpedienteDto expediente)
        {
            var repo = new PacienteRepository(_config);
            try
            {
                return repo.GuardarExpediente(expediente);
            }
            catch (Exception ex)
            {
                
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }

        public List<TipoSangreDto> ListarTiposSangre()
        {
            var repo = new PacienteRepository(_config);
            return repo.ListarTiposSangre();
        }

        public List<SexoBiologicoDto> ListarSexosBiologicos()
        {
            var repo = new PacienteRepository(_config);
            return repo.ListarSexosBiologicos();
        }

        public List<IdentidadGeneroDto> ListarIdentidadesGenero()
        {
            var repo = new PacienteRepository(_config);
            return repo.ListarIdentidadesGenero();
        }

        public List<TipoIdentificacionDto> ListarTiposIdentificacion()
        {
            var repo = new PacienteRepository(_config);
            return repo.ListarTiposIdentificacion();
        }
    }
}
