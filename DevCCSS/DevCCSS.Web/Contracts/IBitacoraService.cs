using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "IBitacoraService")]
    public interface IBitacoraService
    {
        [OperationContract]
        Task<List<BitacoraAuditoriaDto>> ListarAuditoriaAsync(int top);

        [OperationContract]
        Task<List<BitacoraErrorDto>> ListarErroresAsync(int top);

        [OperationContract]
        Task<List<BitacoraNotificacionDto>> ListarNotificacionesAsync(int top);
    }
}
