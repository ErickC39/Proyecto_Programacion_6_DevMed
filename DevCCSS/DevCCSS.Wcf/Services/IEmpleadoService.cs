using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IEmpleadoService
    {
        [OperationContract]
        List<EmpleadoDto> Listar();

        [OperationContract]
        EmpleadoDto? ObtenerPorId(int id);

        [OperationContract]
        RespuestaCrud Crear(EmpleadoDto empleado);

        [OperationContract]
        RespuestaCrud Actualizar(EmpleadoDto empleado);

        [OperationContract]
        RespuestaCrud Eliminar(int id);

        [OperationContract]
        RespuestaCrud CambiarEstado(int idEmpleado, bool activo);
    }
}
