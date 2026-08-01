using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "IExamenMedicoService")]
    public interface IExamenMedicoService
    {
        [OperationContract]
        Task<List<ExamenMedicoDto>> ListarAsync();

        [OperationContract]
        Task<List<ExamenMedicoDto>> ListarPorPacienteAsync(int idPaciente);

        [OperationContract]
        Task<ExamenMedicoDto?> ObtenerPorIdAsync(int id);

        [OperationContract]
        Task<List<TipoExamenDto>> ListarTiposExamenAsync();

        [OperationContract]
        Task<List<EstadoExamenDto>> ListarEstadosExamenAsync();

        [OperationContract]
        Task<List<PacienteExamenDto>> ListarPacientesAsync();

        [OperationContract]
        Task<List<MedicoExamenDto>> ListarMedicosAsync();

        [OperationContract]
        Task<RespuestaCrud> CrearAsync(ExamenMedicoDto examen);

        [OperationContract]
        Task<RespuestaCrud> ActualizarAsync(ExamenMedicoDto examen);

        [OperationContract]
        Task<RespuestaCrud> EliminarAsync(int id);
    }
}
