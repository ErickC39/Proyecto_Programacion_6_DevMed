using System.ServiceModel;

namespace DevCCSS.Web.Contracts
{
    [ServiceContract(Namespace = "http://devccss/services", Name = "IUsuarioService")]
    public interface IUsuarioService
    {
        [OperationContract] Task<List<UsuarioDto>> ListarAsync();
        [OperationContract] Task<UsuarioDto?> ObtenerPorIdAsync(int id);
        [OperationContract] Task<RespuestaCrud> CrearAsync(UsuarioDto usuario);
        [OperationContract] Task<RespuestaCrud> ActualizarAsync(UsuarioDto usuario);
        [OperationContract] Task<RespuestaCrud> EliminarAsync(int id);
        [OperationContract] Task<List<RolDto>> ListarRolesAsync();
    }
}
