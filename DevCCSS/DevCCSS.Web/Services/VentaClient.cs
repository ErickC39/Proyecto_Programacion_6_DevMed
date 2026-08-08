using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    public class VentaClient
    {
        private readonly string _url;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VentaClient(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _url = config["Wcf:VentaServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:VentaServiceUrl en appsettings.json");
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<T> EjecutarAsync<T>(Func<IVentaService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas = { MaxArrayLength = 10 * 1024 * 1024, MaxStringContentLength = 10 * 1024 * 1024 }
            };
            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<IVentaService>(binding, endpoint);
            var client = factory.CreateChannel();
            try
            {
                using var _ = AuditoriaHttpHelper.AplicarUsuarioActual((IContextChannel)client, _httpContextAccessor);
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

        public Task<List<VentaDto>> ListarAsync() => EjecutarAsync(c => c.ListarAsync());
        public Task<VentaDto?> ObtenerPorIdAsync(int id) => EjecutarAsync(c => c.ObtenerPorIdAsync(id));
        public Task<List<ProductoDto>> ListarProductosAsync() => EjecutarAsync(c => c.ListarProductosAsync());
        public Task<RespuestaVenta> RegistrarAsync(CrearVentaDto dto) => EjecutarAsync(c => c.RegistrarAsync(dto));
    }
}