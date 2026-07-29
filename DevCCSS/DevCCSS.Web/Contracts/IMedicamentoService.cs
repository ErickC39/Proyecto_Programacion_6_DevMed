using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "IMedicamentoService")]
    public interface IMedicamentoService
    {
        [OperationContract]
        Task<List<MedicamentoDto>> ListarAsync();

        [OperationContract]
        Task<MedicamentoDto?> ObtenerPorIdAsync(int id);

        [OperationContract]
        Task<RespuestaCrud> CrearAsync(MedicamentoDto medicamento);

        [OperationContract]
        Task<RespuestaCrud> ActualizarAsync(MedicamentoDto medicamento);

        [OperationContract]
        Task<RespuestaCrud> EliminarAsync(int id);
    }
}