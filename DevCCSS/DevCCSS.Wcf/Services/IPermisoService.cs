using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IPermisoService
    {
        [OperationContract]
        List<RolPermisoDto> Listar();
    }
}
