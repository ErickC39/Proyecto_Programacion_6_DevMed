using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    public class ExamenMedicoClient
    {
        private readonly string _url;

        public ExamenMedicoClient(IConfiguration config)
        {
            _url = config["Wcf:ExamenMedicoServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:ExamenMedicoServiceUrl en appsettings.json");
        }

        private async Task<T> EjecutarAsync<T>(Func<IExamenMedicoService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding();
            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<IExamenMedicoService>(binding, endpoint);
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

        public Task<List<ExamenMedicoDto>> ListarAsync() =>
            EjecutarAsync(c => c.ListarAsync());

        public Task<List<ExamenMedicoDto>> ListarPorPacienteAsync(int idPaciente) =>
            EjecutarAsync(c => c.ListarPorPacienteAsync(idPaciente));

        public Task<ExamenMedicoDto?> ObtenerPorIdAsync(int id) =>
            EjecutarAsync(c => c.ObtenerPorIdAsync(id));

        public Task<List<TipoExamenDto>> ListarTiposExamenAsync() =>
            EjecutarAsync(c => c.ListarTiposExamenAsync());

        public Task<List<EstadoExamenDto>> ListarEstadosExamenAsync() =>
            EjecutarAsync(c => c.ListarEstadosExamenAsync());

        public Task<List<PacienteExamenDto>> ListarPacientesAsync() =>
            EjecutarAsync(c => c.ListarPacientesAsync());

        public Task<List<MedicoExamenDto>> ListarMedicosAsync() =>
            EjecutarAsync(c => c.ListarMedicosAsync());

        public Task<RespuestaCrud> CrearAsync(ExamenMedicoDto examen) =>
            EjecutarAsync(c => c.CrearAsync(examen));

        public Task<RespuestaCrud> ActualizarAsync(ExamenMedicoDto examen) =>
            EjecutarAsync(c => c.ActualizarAsync(examen));

        public Task<RespuestaCrud> EliminarAsync(int id) =>
            EjecutarAsync(c => c.EliminarAsync(id));
    }
}
