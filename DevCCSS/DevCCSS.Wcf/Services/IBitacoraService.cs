using CoreWCF;
using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IBitacoraService
    {
        [OperationContract]
        List<BitacoraAuditoriaDto> ListarAuditoria(int top);

        [OperationContract]
        List<BitacoraErrorDto> ListarErrores(int top);

        [OperationContract]
        List<BitacoraNotificacionDto> ListarNotificaciones(int top);
    }
}
