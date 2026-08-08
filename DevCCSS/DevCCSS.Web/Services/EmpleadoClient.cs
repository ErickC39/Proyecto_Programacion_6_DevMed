using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    public class EmpleadoClient
    {
        private readonly string _url;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public EmpleadoClient(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _url = config["Wcf:EmpleadoServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:EmpleadoServiceUrl en appsettings.json");
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<T> EjecutarAsync<T>(Func<IEmpleadoService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas = { MaxArrayLength = 10 * 1024 * 1024, MaxStringContentLength = 10 * 1024 * 1024 }
            };
            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<IEmpleadoService>(binding, endpoint);
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

        public Task<List<EmpleadoDto>> ListarAsync() => EjecutarAsync(c => c.ListarAsync());
        public Task<EmpleadoDto?> ObtenerPorIdAsync(int id) => EjecutarAsync(c => c.ObtenerPorIdAsync(id));
        public Task<RespuestaCrud> CrearAsync(EmpleadoDto x) => EjecutarAsync(c => c.CrearAsync(x));
        public Task<RespuestaCrud> ActualizarAsync(EmpleadoDto x) => EjecutarAsync(c => c.ActualizarAsync(x));
        public Task<RespuestaCrud> EliminarAsync(int id) => EjecutarAsync(c => c.EliminarAsync(id));
        public Task<RespuestaCrud> CambiarEstadoAsync(int idEmpleado, bool activo) => EjecutarAsync(c => c.CambiarEstadoAsync(idEmpleado, activo));
    }
}
