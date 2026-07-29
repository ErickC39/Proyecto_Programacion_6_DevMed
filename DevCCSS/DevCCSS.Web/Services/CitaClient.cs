using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    // Cliente del WS de Citas. Mismo modelo que PacienteClient: abre el canal, llama y lo cierra.
    public class CitaClient
    {
        private readonly string _url;

        public CitaClient(IConfiguration config)
        {
            _url = config["Wcf:CitaServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:CitaServiceUrl en appsettings.json");
        }

        // Helper para no repetir el abrir/cerrar canal en cada metodo.
        private async Task<T> EjecutarAsync<T>(Func<ICitaService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas = { MaxArrayLength = 10 * 1024 * 1024, MaxStringContentLength = 10 * 1024 * 1024 }
            };
            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<ICitaService>(binding, endpoint);
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

        public Task<List<CitaDto>> ListarAsync() => EjecutarAsync(c => c.ListarAsync());
        public Task<CitaDto?> ObtenerPorIdAsync(int id) => EjecutarAsync(c => c.ObtenerPorIdAsync(id));
        public Task<RespuestaCita> AgendarAsync(AgendarCitaDto dto) => EjecutarAsync(c => c.AgendarAsync(dto));
        public Task<RespuestaCita> AgendarEmergenciaAsync(AgendarEmergenciaDto dto) => EjecutarAsync(c => c.AgendarEmergenciaAsync(dto));
        public Task<RespuestaCita> RegistrarLlegadaAsync(int idCita) => EjecutarAsync(c => c.RegistrarLlegadaAsync(idCita));
        public Task<RespuestaCita> IniciarAtencionAsync(int idCita) => EjecutarAsync(c => c.IniciarAtencionAsync(idCita));
        public Task<RespuestaCita> CancelarAsync(int idCita) => EjecutarAsync(c => c.CancelarAsync(idCita));
        public Task<RespuestaCita> EliminarAsync(int idCita) => EjecutarAsync(c => c.EliminarAsync(idCita));

        public Task<RespuestaCita> FinalizarAsync(FinalizarCitaDto dto) => EjecutarAsync(c => c.FinalizarAsync(dto));
    }
}