using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    public class EnfermedadClient
    {
        private readonly string _url;
        public EnfermedadClient(IConfiguration config)
        {
            _url = config["Wcf:EnfermedadServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:EnfermedadServiceUrl en appsettings.json");
        }

        private async Task<T> EjecutarAsync<T>(Func<IEnfermedadService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas = { MaxArrayLength = 10 * 1024 * 1024, MaxStringContentLength = 10 * 1024 * 1024 }
            };
            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<IEnfermedadService>(binding, endpoint);
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

        public Task<List<EnfermedadDto>> ListarAsync() => EjecutarAsync(c => c.ListarAsync());
        public Task<EnfermedadDto?> ObtenerPorIdAsync(int id) => EjecutarAsync(c => c.ObtenerPorIdAsync(id));
        public Task<RespuestaCrud> CrearAsync(EnfermedadDto x) => EjecutarAsync(c => c.CrearAsync(x));
        public Task<RespuestaCrud> ActualizarAsync(EnfermedadDto x) => EjecutarAsync(c => c.ActualizarAsync(x));
        public Task<RespuestaCrud> EliminarAsync(int id) => EjecutarAsync(c => c.EliminarAsync(id));

        public Task<List<TratamientoEnfermedadDto>> ListarMedicamentosAsync(int idEnfermedad) => EjecutarAsync(c => c.ListarMedicamentosAsync(idEnfermedad));
        public Task<RespuestaCrud> AsignarMedicamentoAsync(int idEnfermedad, int idMedicamento, string? observacion) => EjecutarAsync(c => c.AsignarMedicamentoAsync(idEnfermedad, idMedicamento, observacion));
        public Task<RespuestaCrud> QuitarMedicamentoAsync(int idEnfermedad, int idMedicamento) => EjecutarAsync(c => c.QuitarMedicamentoAsync(idEnfermedad, idMedicamento));
    }
}
