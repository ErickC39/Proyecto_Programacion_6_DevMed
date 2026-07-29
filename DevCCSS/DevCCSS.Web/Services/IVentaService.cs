using System.ServiceModel;
using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IVentaService
    {
        [OperationContract]
        Task<List<VentaDto>> ListarAsync();

        [OperationContract]
        Task<VentaDto?> ObtenerPorIdAsync(int id);

        [OperationContract]
        Task<List<ProductoDto>> ListarProductosAsync();

        [OperationContract]
        Task<RespuestaVenta> RegistrarAsync(CrearVentaDto dto);
    }
}