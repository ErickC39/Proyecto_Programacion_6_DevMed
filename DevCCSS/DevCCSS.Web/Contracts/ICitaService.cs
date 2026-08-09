using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    // Mismo Namespace y Name que el servidor para que el SOAP haga match.
    [ServiceContract(Namespace = "http://devccss/services", Name = "ICitaService")]
    public interface ICitaService
    {
        [OperationContract]
        Task<List<CitaDto>> ListarAsync();

        [OperationContract]
        Task<CitaDto?> ObtenerPorIdAsync(int id);

        [OperationContract]
        Task<RespuestaCita> AgendarAsync(AgendarCitaDto dto);

        [OperationContract]
        Task<RespuestaCita> AgendarEmergenciaAsync(AgendarEmergenciaDto dto);

        [OperationContract]
        Task<RespuestaCita> RegistrarLlegadaAsync(int idCita);

        [OperationContract]
        Task<RespuestaCita> IniciarAtencionAsync(int idCita, int? idHabitacion);

        [OperationContract]
        Task<RespuestaCita> CancelarAsync(int idCita);

        [OperationContract]
        Task<RespuestaCita> EliminarAsync(int idCita);

        [OperationContract]
        Task<RespuestaCita> FinalizarAsync(FinalizarCitaDto dto);

    }
}