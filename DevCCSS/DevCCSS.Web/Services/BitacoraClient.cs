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
            // Buffer mas holgado que el resto de los clientes WCF: al ya no
            // purgarse Bitacora_Auditoria automaticamente, el historial crece
            // sin limite y conviene dejar margen para lotes de auditoria mas
            // grandes sin que el transporte falle por tamano de mensaje.
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 50 * 1024 * 1024,
                MaxBufferSize = 50 * 1024 * 1024,
                ReaderQuotas = { MaxArrayLength = 50 * 1024 * 1024, MaxStringContentLength = 50 * 1024 * 1024 }
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

        public Task<List<BitacoraAuditoriaDto>> ListarAuditoriaAsync(int top = 100) => EjecutarAsync(c => c.ListarAuditoriaAsync(top));
        public Task<List<BitacoraErrorDto>> ListarErroresAsync(int top = 200) => EjecutarAsync(c => c.ListarErroresAsync(top));
        public Task<List<BitacoraNotificacionDto>> ListarNotificacionesAsync(int top = 200) => EjecutarAsync(c => c.ListarNotificacionesAsync(top));
    }
}
