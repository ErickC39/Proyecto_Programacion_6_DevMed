using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class HabitacionService : IHabitacionService
    {
        private readonly IConfiguration _config;

        public HabitacionService(IConfiguration config)
        {
            _config = config;
        }

        public List<HabitacionDto> Listar() => new HabitacionRepository(_config).Listar();

        public HabitacionDto? ObtenerPorId(int id) => new HabitacionRepository(_config).ObtenerPorId(id);

        public List<TipoHabitacionDto> ListarTiposHabitacion() => new HabitacionRepository(_config).ListarTiposHabitacion();

        public List<EstadoHabitacionDto> ListarEstadosHabitacion() => new HabitacionRepository(_config).ListarEstadosHabitacion();

        public List<PacienteHabitacionDto> ListarPacientes() => new HabitacionRepository(_config).ListarPacientes();

        public List<EmpleadoHabitacionDto> ListarEmpleados() => new HabitacionRepository(_config).ListarEmpleados();

        public RespuestaCrud Crear(HabitacionDto habitacion)
        {
            try { return new HabitacionRepository(_config).Crear(habitacion); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }

        public RespuestaCrud Actualizar(HabitacionDto habitacion)
        {
            try { return new HabitacionRepository(_config).Actualizar(habitacion); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }

        public RespuestaCrud Asignar(AsignarHabitacionDto asignacion)
        {
            try { return new HabitacionRepository(_config).Asignar(asignacion); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }

        public RespuestaCrud Liberar(LiberarHabitacionDto liberacion)
        {
            try { return new HabitacionRepository(_config).Liberar(liberacion); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }

        public RespuestaCrud Eliminar(int id)
        {
            try { return new HabitacionRepository(_config).Eliminar(id); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
    }
}
