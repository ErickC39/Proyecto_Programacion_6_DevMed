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

        public RespuestaCrud Crear(EspecialidadDto especialidad)
        {
            try
            {
                return new EspecialidadRepository(_config).Crear(especialidad);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 2601 || ex.Number == 2627)
            {
                return new RespuestaCrud { Ok = false, Mensaje = "Ya existe una especialidad con ese nombre." };
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }
    }
}
