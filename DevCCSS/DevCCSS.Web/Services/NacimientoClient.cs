using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    public class NacimientoClient
    {
        private readonly string _url;
        public NacimientoClient(IConfiguration config)
        {
            _url = config["Wcf:NacimientoServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:NacimientoServiceUrl en appsettings.json");
        }

        private async Task<T> EjecutarAsync<T>(Func<INacimientoService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas = { MaxArrayLength = 10 * 1024 * 1024, MaxStringContentLength = 10 * 1024 * 1024 }
            };
            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<INacimientoService>(binding, endpoint);
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

        public Task<List<NacimientoDto>> ListarAsync() => EjecutarAsync(c => c.ListarAsync());
        public Task<RespuestaCrud> RegistrarAsync(NacimientoDto n) => EjecutarAsync(c => c.RegistrarAsync(n));
        public Task<RespuestaCrud> RegistrarCompletoAsync(RegistrarNacimientoDto n) => EjecutarAsync(c => c.RegistrarCompletoAsync(n));
        public Task<NacimientoDetalleDto?> ObtenerDetalleAsync(int idPaciente) => EjecutarAsync(c => c.ObtenerDetalleAsync(idPaciente));
        public Task<RespuestaCrud> ActualizarAsync(RegistrarNacimientoDto n) => EjecutarAsync(c => c.ActualizarAsync(n));
        public Task<RespuestaCrud> EliminarAsync(int idPaciente) => EjecutarAsync(c => c.EliminarAsync(idPaciente));
    }
}