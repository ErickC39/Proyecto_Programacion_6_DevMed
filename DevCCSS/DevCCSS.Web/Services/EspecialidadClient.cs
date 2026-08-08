using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    public class EspecialidadClient
    {
        private readonly string _url;

        public EspecialidadClient(IConfiguration config)
        {
            _url = config["Wcf:EspecialidadServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:EspecialidadServiceUrl en appsettings.json");
        }

        private async Task<T> EjecutarAsync<T>(Func<IEspecialidadService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas =
                {
                    MaxArrayLength = 10 * 1024 * 1024,
                    MaxStringContentLength = 10 * 1024 * 1024
                }
            };

            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<IEspecialidadService>(binding, endpoint);
            var client = factory.CreateChannel();

            try
            {
                var resultado = await accion(client);
                ((IClientChannel)client).Close();
                factory.Close();
                return resultado;
            }
            catch
            {
                ((IClientChannel)client).Abort();
                factory.Abort();
                throw;
            }
        }

        public Task<List<EspecialidadDto>> ListarAsync()
            => EjecutarAsync(c => c.ListarAsync());
    }
}
