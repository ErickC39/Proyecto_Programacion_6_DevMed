using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    // Mismo Namespace y Name que el servidor para que el SOAP haga match.
    [ServiceContract(Namespace = "http://devccss/services", Name = "ISeguridadService")]
    public interface ISeguridadService
    {
        [OperationContract]
        Task<string> PingAsync();

        [OperationContract]
        Task<LoginResponse> ValidarLoginAsync(string username, string password);
    }
}
