using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    // Cliente del WS de Pacientes. ESTE es el modelo a copiar por modulo.
    public class PacienteClient
    {
        private readonly string _url;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PacienteClient(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _url = config["Wcf:PacienteServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:PacienteServiceUrl en appsettings.json");
            _httpContextAccessor = httpContextAccessor;
        }

        // Helper para no repetir el abrir/cerrar canal en cada metodo.
        private async Task<T> EjecutarAsync<T>(Func<IPacienteService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas = { MaxArrayLength = 10 * 1024 * 1024, MaxStringContentLength = 10 * 1024 * 1024 }
            };
            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<IPacienteService>(binding, endpoint);
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

        public Task<List<PacienteDto>> ListarAsync() => EjecutarAsync(c => c.ListarAsync());
        public Task<PacienteDto?> ObtenerPorIdAsync(int id) => EjecutarAsync(c => c.ObtenerPorIdAsync(id));
        public Task<RespuestaCrud> CrearAsync(PacienteDto p) => EjecutarAsync(c => c.CrearAsync(p));
        public Task<RespuestaCrud> ActualizarAsync(PacienteDto p) => EjecutarAsync(c => c.ActualizarAsync(p));
        public Task<RespuestaCrud> EliminarAsync(int id) => EjecutarAsync(c => c.EliminarAsync(id));
        public Task<RespuestaCrud> GuardarExpedienteAsync(ExpedienteDto e) => EjecutarAsync(c => c.GuardarExpedienteAsync(e));
        public Task<List<TipoSangreDto>> ListarTiposSangreAsync() => EjecutarAsync(c => c.ListarTiposSangreAsync());

        public Task<List<SexoBiologicoDto>> ListarSexosBiologicosAsync() => EjecutarAsync(c => c.ListarSexosBiologicosAsync());
        public Task<List<IdentidadGeneroDto>> ListarIdentidadesGeneroAsync() => EjecutarAsync(c => c.ListarIdentidadesGeneroAsync());
        public Task<List<TipoIdentificacionDto>> ListarTiposIdentificacionAsync() => EjecutarAsync(c => c.ListarTiposIdentificacionAsync());
    }
}
