using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "IEspecialidadService")]
    public interface IEspecialidadService
    {
        [OperationContract]
        Task<List<EspecialidadDto>> ListarAsync();

        [OperationContract]
        Task<RespuestaCrud> CrearAsync(EspecialidadDto especialidad);
    }
}
