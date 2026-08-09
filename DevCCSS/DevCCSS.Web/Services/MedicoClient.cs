using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    public class MedicoClient
    {
        private readonly string _url;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MedicoClient(IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _url = config["Wcf:MedicoServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:MedicoServiceUrl en appsettings.json");
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<T> EjecutarAsync<T>(Func<IMedicoService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas =
                {
                    MaxArrayLength = 10 * 1024 * 1024,
                    MaxStringContentLength = 10 * 1024 * 1024
                }
            };

            var endpoint = new EndpointAddress(_url);

            var factory = new ChannelFactory<IMedicoService>(
                binding,
                endpoint);

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

        public Task<List<MedicoDto>> ListarAsync()
            => EjecutarAsync(c => c.ListarAsync());

        public Task<MedicoDto?> ObtenerPorIdAsync(int idMedico)
            => EjecutarAsync(c => c.ObtenerPorIdAsync(idMedico));

        public Task<RespuestaCrud> CrearAsync(MedicoDto medico)
            => EjecutarAsync(c => c.CrearAsync(medico));

        public Task<RespuestaCrud> ActualizarAsync(MedicoDto medico)
            => EjecutarAsync(c => c.ActualizarAsync(medico));

        public Task<RespuestaCrud> EliminarAsync(int idMedico)
            => EjecutarAsync(c => c.EliminarAsync(idMedico));

        public Task<List<HorarioMedicoDto>> ListarHorariosAsync(int idMedico)
            => EjecutarAsync(c => c.ListarHorariosAsync(idMedico));

        public Task<List<CitaDto>> ListarCitasAsignadasAsync(int idMedico)
            => EjecutarAsync(c => c.ListarCitasAsignadasAsync(idMedico));

        public Task<RespuestaCrud> AgregarHorarioAsync(HorarioMedicoDto horario)
            => EjecutarAsync(c => c.AgregarHorarioAsync(horario));
    }
}
