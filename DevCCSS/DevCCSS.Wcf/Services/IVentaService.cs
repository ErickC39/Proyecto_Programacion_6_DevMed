using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IVentaService
    {
        [OperationContract] List<VentaDto> Listar();
        [OperationContract] VentaDto? ObtenerPorId(int id);
        [OperationContract] List<ProductoDto> ListarProductos();
        [OperationContract] RespuestaVenta Registrar(CrearVentaDto dto);
    }
}