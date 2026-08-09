using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "IPacienteService")]
    public interface IPacienteService
    {
        [OperationContract]
        Task<List<PacienteDto>> ListarAsync();

        [OperationContract]
        Task<PacienteDto?> ObtenerPorIdAsync(int id);

        [OperationContract]
        Task<RespuestaCrud> CrearAsync(PacienteDto paciente);

        [OperationContract]
        Task<RespuestaCrud> ActualizarAsync(PacienteDto paciente);

        [OperationContract]
        Task<RespuestaCrud> EliminarAsync(int id);

        [OperationContract]
        Task<RespuestaCrud> GuardarExpedienteAsync(ExpedienteDto expediente);

        [OperationContract]
        Task<List<TipoSangreDto>> ListarTiposSangreAsync();

        [OperationContract]
        Task<List<SexoBiologicoDto>> ListarSexosBiologicosAsync();

        [OperationContract]
        Task<List<IdentidadGeneroDto>> ListarIdentidadesGeneroAsync();

        [OperationContract]
        Task<List<TipoIdentificacionDto>> ListarTiposIdentificacionAsync();
    }
}
