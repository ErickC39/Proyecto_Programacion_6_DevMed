using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    public class PermisoClient
    {
        private readonly string _url;
        public PermisoClient(IConfiguration config)
        {
            _url = config["Wcf:PermisoServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:PermisoServiceUrl en appsettings.json");
        }

        private async Task<T> EjecutarAsync<T>(Func<IPermisoService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas = { MaxArrayLength = 10 * 1024 * 1024, MaxStringContentLength = 10 * 1024 * 1024 }
            };
            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<IPermisoService>(binding, endpoint);
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

        public Task<List<RolPermisoDto>> ListarAsync() => EjecutarAsync(c => c.ListarAsync());
    }
}
