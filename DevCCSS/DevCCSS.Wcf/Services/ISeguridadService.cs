using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface ISeguridadService
    {
        // metodo de prueba: sirve para verificar que el WS esta arriba
        [OperationContract]
        string Ping();

        [OperationContract]
        LoginResponse ValidarLogin(string username, string password);
    }
}
