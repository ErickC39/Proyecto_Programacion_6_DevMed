using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "IHabitacionService")]
    public interface IHabitacionService
    {
        [OperationContract]
        Task<List<HabitacionDto>> ListarAsync();

        [OperationContract]
        Task<HabitacionDto?> ObtenerPorIdAsync(int id);

        [OperationContract]
        Task<List<TipoHabitacionDto>> ListarTiposHabitacionAsync();

        [OperationContract]
        Task<List<EstadoHabitacionDto>> ListarEstadosHabitacionAsync();

        [OperationContract]
        Task<List<PacienteHabitacionDto>> ListarPacientesAsync();

        [OperationContract]
        Task<List<EmpleadoHabitacionDto>> ListarEmpleadosAsync();

        [OperationContract]
        Task<RespuestaCrud> CrearAsync(HabitacionDto habitacion);

        [OperationContract]
        Task<RespuestaCrud> ActualizarAsync(HabitacionDto habitacion);

        [OperationContract]
        Task<RespuestaCrud> AsignarAsync(AsignarHabitacionDto asignacion);

        [OperationContract]
        Task<RespuestaCrud> LiberarAsync(LiberarHabitacionDto liberacion);

        [OperationContract]
        Task<RespuestaCrud> EliminarAsync(int id);

        [OperationContract]
        Task<List<OcupanteHabitacionDto>> ListarOcupantesActivosAsync(int idHabitacion);
    }
}
