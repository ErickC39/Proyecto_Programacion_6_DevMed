using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    public class HabitacionClient
    {
        private readonly string _url;

        public HabitacionClient(IConfiguration config)
        {
            _url = config["Wcf:HabitacionServiceUrl"]
                ?? throw new InvalidOperationException("Falta Wcf:HabitacionServiceUrl en appsettings.json");
        }

        private async Task<T> EjecutarAsync<T>(Func<IHabitacionService, Task<T>> accion)
        {
            var binding = new BasicHttpBinding
            {
                MaxReceivedMessageSize = 10 * 1024 * 1024,
                MaxBufferSize = 10 * 1024 * 1024,
                ReaderQuotas = { MaxArrayLength = 10 * 1024 * 1024, MaxStringContentLength = 10 * 1024 * 1024 }
            };
            var endpoint = new EndpointAddress(_url);
            var factory = new ChannelFactory<IHabitacionService>(binding, endpoint);
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

        public Task<List<HabitacionDto>> ListarAsync() =>
            EjecutarAsync(c => c.ListarAsync());

        public Task<HabitacionDto?> ObtenerPorIdAsync(int id) =>
            EjecutarAsync(c => c.ObtenerPorIdAsync(id));

        public Task<List<TipoHabitacionDto>> ListarTiposHabitacionAsync() =>
            EjecutarAsync(c => c.ListarTiposHabitacionAsync());

        public Task<List<EstadoHabitacionDto>> ListarEstadosHabitacionAsync() =>
            EjecutarAsync(c => c.ListarEstadosHabitacionAsync());

        public Task<List<PacienteHabitacionDto>> ListarPacientesAsync() =>
            EjecutarAsync(c => c.ListarPacientesAsync());

        public Task<List<EmpleadoHabitacionDto>> ListarEmpleadosAsync() =>
            EjecutarAsync(c => c.ListarEmpleadosAsync());

        public Task<RespuestaCrud> CrearAsync(HabitacionDto habitacion) =>
            EjecutarAsync(c => c.CrearAsync(habitacion));

        public Task<RespuestaCrud> ActualizarAsync(HabitacionDto habitacion) =>
            EjecutarAsync(c => c.ActualizarAsync(habitacion));

        public Task<RespuestaCrud> AsignarAsync(AsignarHabitacionDto asignacion) =>
            EjecutarAsync(c => c.AsignarAsync(asignacion));

        public Task<RespuestaCrud> LiberarAsync(LiberarHabitacionDto liberacion) =>
            EjecutarAsync(c => c.LiberarAsync(liberacion));

        public Task<RespuestaCrud> EliminarAsync(int id) =>
            EjecutarAsync(c => c.EliminarAsync(id));
    }
}
