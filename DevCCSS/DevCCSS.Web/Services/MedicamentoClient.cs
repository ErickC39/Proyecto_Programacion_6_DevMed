using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    public class MedicamentoClient
    {
        private readonly string _url;

        public MedicamentoClient(IConfiguration config)
        {
            _url = config["Wcf:MedicamentoServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:MedicamentoServiceUrl en appsettings.json");
        }

        private async Task<T> EjecutarAsync<T>(Func<IMedicamentoService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas = { MaxArrayLength = 10 * 1024 * 1024, MaxStringContentLength = 10 * 1024 * 1024 }
            };
            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<IMedicamentoService>(binding, endpoint);
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

        public Task<List<MedicamentoDto>> ListarAsync() => EjecutarAsync(c => c.ListarAsync());
        public Task<MedicamentoDto?> ObtenerPorIdAsync(int id) => EjecutarAsync(c => c.ObtenerPorIdAsync(id));
        public Task<RespuestaCrud> CrearAsync(MedicamentoDto m) => EjecutarAsync(c => c.CrearAsync(m));
        public Task<RespuestaCrud> ActualizarAsync(MedicamentoDto m) => EjecutarAsync(c => c.ActualizarAsync(m));
        public Task<RespuestaCrud> EliminarAsync(int id) => EjecutarAsync(c => c.EliminarAsync(id));
    }
}