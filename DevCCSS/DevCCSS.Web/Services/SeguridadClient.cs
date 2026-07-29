using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    // Cliente del WS de Seguridad. Abre el canal, llama y lo cierra.
    public class SeguridadClient
    {
        private readonly string _url;

        public SeguridadClient(IConfiguration config)
        {
            _url = config["Wcf:SeguridadServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:SeguridadServiceUrl en appsettings.json");
        }

        public async Task<LoginResponse> ValidarLoginAsync(string username, string password)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas = { MaxArrayLength = 10 * 1024 * 1024, MaxStringContentLength = 10 * 1024 * 1024 }
            };
            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<ISeguridadService>(binding, endpoint);
            var client = factory.CreateChannel();

            try
            {
                var result = await client.ValidarLoginAsync(username, password);
                ((IClientChannel)client).Close();
                factory.Close();
                return result;
            }
            catch
            {
                ((IClientChannel)client).Abort();
                factory.Abort();
                throw;
            }
        }
    }
}
