using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "IEnfermedadService")]
    public interface IEnfermedadService
    {
        [OperationContract]
        Task<List<EnfermedadDto>> ListarAsync();

        [OperationContract]
        Task<EnfermedadDto?> ObtenerPorIdAsync(int id);

        [OperationContract]
        Task<RespuestaCrud> CrearAsync(EnfermedadDto enfermedad);

        [OperationContract]
        Task<RespuestaCrud> ActualizarAsync(EnfermedadDto enfermedad);

        [OperationContract]
        Task<RespuestaCrud> EliminarAsync(int id);

        [OperationContract]
        Task<List<TratamientoEnfermedadDto>> ListarMedicamentosAsync(int idEnfermedad);

        [OperationContract]
        Task<RespuestaCrud> AsignarMedicamentoAsync(int idEnfermedad, int idMedicamento, string? observacion);

        [OperationContract]
        Task<RespuestaCrud> QuitarMedicamentoAsync(int idEnfermedad, int idMedicamento);
    }
}
