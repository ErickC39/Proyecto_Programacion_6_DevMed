using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IUsuarioService
    {
        [OperationContract] List<UsuarioDto> Listar();
        [OperationContract] UsuarioDto? ObtenerPorId(int id);
        [OperationContract] RespuestaCrud Crear(UsuarioDto usuario);
        [OperationContract] RespuestaCrud Actualizar(UsuarioDto usuario);
        [OperationContract] RespuestaCrud Eliminar(int id);
        [OperationContract] List<RolDto> ListarRoles();
    }
}
