using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IHabitacionService
    {
        [OperationContract]
        List<HabitacionDto> Listar();

        [OperationContract]
        HabitacionDto? ObtenerPorId(int id);

        [OperationContract]
        List<TipoHabitacionDto> ListarTiposHabitacion();

        [OperationContract]
        List<EstadoHabitacionDto> ListarEstadosHabitacion();

        [OperationContract]
        List<PacienteHabitacionDto> ListarPacientes();

        [OperationContract]
        List<EmpleadoHabitacionDto> ListarEmpleados();

        [OperationContract]
        RespuestaCrud Crear(HabitacionDto habitacion);

        [OperationContract]
        RespuestaCrud Actualizar(HabitacionDto habitacion);

        [OperationContract]
        RespuestaCrud Asignar(AsignarHabitacionDto asignacion);

        [OperationContract]
        RespuestaCrud Liberar(LiberarHabitacionDto liberacion);

        [OperationContract]
        RespuestaCrud Eliminar(int id);

        [OperationContract]
        List<OcupanteHabitacionDto> ListarOcupantesActivos(int idHabitacion);
    }
}
