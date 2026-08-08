using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "IMedicoService")]
    public interface IMedicoService
    {
        [OperationContract]
        Task<List<MedicoDto>> ListarAsync();
        [OperationContract]
        Task<MedicoDto?> ObtenerPorIdAsync(int idMedico);

        [OperationContract]
        Task<EmpleadoDto?> BuscarEmpleadoAsync(string identificacion);

        [OperationContract]
        Task<RespuestaCrud> CrearAsync(MedicoDto medico);
        [OperationContract]
        Task<RespuestaCrud> ActualizarAsync(MedicoDto medico);
        [OperationContract]
        Task<RespuestaCrud> AgregarHorarioAsync(HorarioMedicoDto horario);
        [OperationContract]
        Task<List<HorarioMedicoDto>> ListarHorariosAsync(int idMedico);

        [OperationContract]
        Task<List<CitaDto>> ListarCitasAsignadasAsync(int idMedico);
    }
}
