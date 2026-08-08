using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "IVisitanteService")]
    public interface IVisitanteService
    {
        [OperationContract]
        Task<List<VisitanteDto>> ListarAsync();

        [OperationContract]
        Task<VisitanteDto?> ObtenerPorIdAsync(int id);

        [OperationContract]
        Task<RespuestaCrud> CrearAsync(VisitanteDto visitante);

        [OperationContract]
        Task<RespuestaCrud> ActualizarAsync(VisitanteDto visitante);

        [OperationContract]
        Task<RespuestaCrud> EliminarAsync(int id);

        [OperationContract]
        Task<RespuestaCrud> RegistrarSalidaAsync(int idVisita, DateTime? fechaHoraSalida);
    }
}
