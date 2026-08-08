using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "IEmpleadoService")]
    public interface IEmpleadoService
    {
        [OperationContract]
        Task<List<EmpleadoDto>> ListarAsync();

        [OperationContract]
        Task<EmpleadoDto?> ObtenerPorIdAsync(int id);

        [OperationContract]
        Task<RespuestaCrud> CrearAsync(EmpleadoDto empleado);

        [OperationContract]
        Task<RespuestaCrud> ActualizarAsync(EmpleadoDto empleado);

        [OperationContract]
        Task<RespuestaCrud> EliminarAsync(int id);

        [OperationContract]
        Task<RespuestaCrud> CambiarEstadoAsync(int idEmpleado, bool activo);
    }
}
