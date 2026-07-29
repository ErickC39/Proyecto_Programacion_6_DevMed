using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "IPermisoService")]
    public interface IPermisoService
    {
        [OperationContract]
        Task<List<RolPermisoDto>> ListarAsync();
    }
}
