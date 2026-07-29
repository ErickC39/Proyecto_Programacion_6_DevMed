using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IInventarioService
    {
        [OperationContract]
        List<ProductoDto> Listar();

        [OperationContract]
        ProductoDto? ObtenerPorId(int id);

        [OperationContract]
        RespuestaCrud Crear(ProductoDto producto);

        [OperationContract]
        RespuestaCrud Actualizar(ProductoDto producto);

        [OperationContract]
        RespuestaCrud Eliminar(int id);
    }
}
