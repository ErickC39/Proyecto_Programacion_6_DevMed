using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IConfiguration _config;
        public UsuarioService(IConfiguration config) { _config = config; }

        public List<UsuarioDto> Listar() => new UsuarioAdminRepository(_config).Listar();
        public UsuarioDto? ObtenerPorId(int id) => new UsuarioAdminRepository(_config).ObtenerPorId(id);
        public List<RolDto> ListarRoles() => new UsuarioAdminRepository(_config).ListarRoles();

        public RespuestaCrud Crear(UsuarioDto usuario)
        {
            try { return new UsuarioAdminRepository(_config).Crear(usuario); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
        public RespuestaCrud Actualizar(UsuarioDto usuario)
        {
            try { return new UsuarioAdminRepository(_config).Actualizar(usuario); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
        public RespuestaCrud Eliminar(int id)
        {
            try { return new UsuarioAdminRepository(_config).Eliminar(id); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
    }
}
