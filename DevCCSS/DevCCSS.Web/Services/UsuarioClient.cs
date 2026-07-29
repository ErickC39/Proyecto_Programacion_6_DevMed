using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    public class UsuarioClient
    {
        private readonly string _url;
        public UsuarioClient(IConfiguration config)
        {
            _url = config["Wcf:UsuarioServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:UsuarioServiceUrl en appsettings.json");
        }

        private async Task<T> EjecutarAsync<T>(Func<IUsuarioService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas = { MaxArrayLength = 10 * 1024 * 1024, MaxStringContentLength = 10 * 1024 * 1024 }
            };
            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<IUsuarioService>(binding, endpoint);
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

        public Task<List<UsuarioDto>> ListarAsync() => EjecutarAsync(c => c.ListarAsync());
        public Task<UsuarioDto?> ObtenerPorIdAsync(int id) => EjecutarAsync(c => c.ObtenerPorIdAsync(id));
        public Task<RespuestaCrud> CrearAsync(UsuarioDto u) => EjecutarAsync(c => c.CrearAsync(u));
        public Task<RespuestaCrud> ActualizarAsync(UsuarioDto u) => EjecutarAsync(c => c.ActualizarAsync(u));
        public Task<RespuestaCrud> EliminarAsync(int id) => EjecutarAsync(c => c.EliminarAsync(id));
        public Task<List<RolDto>> ListarRolesAsync() => EjecutarAsync(c => c.ListarRolesAsync());
    }
}
