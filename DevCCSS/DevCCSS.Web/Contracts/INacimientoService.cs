using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "INacimientoService")]
    public interface INacimientoService
    {
        [OperationContract]
        Task<List<NacimientoDto>> ListarAsync();

        [OperationContract]
        Task<RespuestaCrud> RegistrarAsync(NacimientoDto nacimiento);

        [OperationContract]
        Task<RespuestaCrud> RegistrarCompletoAsync(RegistrarNacimientoDto nacimiento);

        [OperationContract]
        Task<NacimientoDetalleDto?> ObtenerDetalleAsync(int idPaciente);

        [OperationContract]
        Task<RespuestaCrud> ActualizarAsync(RegistrarNacimientoDto nacimiento);

        [OperationContract]
        Task<RespuestaCrud> EliminarAsync(int idPaciente);
    }
}