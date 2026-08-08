using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IEspecialidadService
    {
        [OperationContract]
        List<EspecialidadDto> Listar();
    }
}
