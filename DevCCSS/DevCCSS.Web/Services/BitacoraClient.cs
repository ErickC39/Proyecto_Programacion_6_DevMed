using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    public class BitacoraClient
    {
        private readonly string _url;
        public BitacoraClient(IConfiguration config)
        {
            _url = config["Wcf:BitacoraServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:BitacoraServiceUrl en appsettings.json");
        }

        private async Task<T> EjecutarAsync<T>(Func<IBitacoraService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas = { MaxArrayLength = 10 * 1024 * 1024, MaxStringContentLength = 10 * 1024 * 1024 }
            };
            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<IBitacoraService>(binding, endpoint);
            var client = factory.CreateChannel();
            try
            {
                var result = await accion(client);
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

        public Task<List<BitacoraAuditoriaDto>> ListarAuditoriaAsync(int top = 200) => EjecutarAsync(c => c.ListarAuditoriaAsync(top));
        public Task<List<BitacoraErrorDto>> ListarErroresAsync(int top = 200) => EjecutarAsync(c => c.ListarErroresAsync(top));
        public Task<List<BitacoraNotificacionDto>> ListarNotificacionesAsync(int top = 200) => EjecutarAsync(c => c.ListarNotificacionesAsync(top));
    }
}
