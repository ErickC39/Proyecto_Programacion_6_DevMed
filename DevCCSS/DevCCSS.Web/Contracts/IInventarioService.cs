using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "IInventarioService")]
    public interface IInventarioService
    {
        [OperationContract]
        Task<List<ProductoDto>> ListarAsync();

        [OperationContract]
        Task<ProductoDto?> ObtenerPorIdAsync(int id);

        [OperationContract]
        Task<RespuestaCrud> CrearAsync(ProductoDto producto);

        [OperationContract]
        Task<RespuestaCrud> ActualizarAsync(ProductoDto producto);

        [OperationContract]
        Task<RespuestaCrud> EliminarAsync(int id);
    }
}
