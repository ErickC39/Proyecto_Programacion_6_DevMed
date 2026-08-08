using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;
using Microsoft.Data.SqlClient;

namespace DevCCSS.Wcf.Services
{
    public class EmpleadoService : IEmpleadoService
    {
        private readonly IConfiguration _config;
        public EmpleadoService(IConfiguration config) { _config = config; }

        public List<EmpleadoDto> Listar() => new EmpleadoRepository(_config).Listar();
        public EmpleadoDto? ObtenerPorId(int id) => new EmpleadoRepository(_config).ObtenerPorId(id);

        public RespuestaCrud Crear(EmpleadoDto empleado)
        {
            try { return new EmpleadoRepository(_config).Crear(empleado); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
        public RespuestaCrud Actualizar(EmpleadoDto empleado)
        {
            try { return new EmpleadoRepository(_config).Actualizar(empleado); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
        public RespuestaCrud Eliminar(int id)
        {
            try
            {
                return new EmpleadoRepository(_config).Eliminar(id);
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                // Violación de llave foránea: el empleado tiene registros dependientes
                // (por ejemplo, citas médicas asignadas como médico).
                return new RespuestaCrud
                {
                    Ok = false,
                    Mensaje = "No se puede eliminar este empleado porque tiene citas medicas. " 
                              
                };
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }

        public RespuestaCrud CambiarEstado(int idEmpleado, bool activo)
        {
            try { return new EmpleadoRepository(_config).CambiarEstado(idEmpleado, activo); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
    }
}
