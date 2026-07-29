using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class SeguridadService : ISeguridadService
    {
        private readonly IConfiguration _config;

        public SeguridadService(IConfiguration config)
        {
            _config = config;
        }

        public string Ping()
        {
            return "DevCCSS WCF arriba y corriendo.";
        }

        public LoginResponse ValidarLogin(string username, string password)
        {
            var repo = new UsuarioRepository(_config);
            var user = repo.GetByUsername(username);

            // 1) que exista y este activo
            if (user is null || !user.Activo)
                return new LoginResponse { Success = false, Message = "Usuario o contrasenia incorrectos." };

            // 2) validar password contra hash + salt
            bool ok = PasswordVerifier.VerifySha512(user.PasswordSalt, user.PasswordHash, password);
            if (!ok)
                return new LoginResponse { Success = false, Message = "Usuario o contrasenia incorrectos." };

            // 3) traer rol(es)
            var roles = repo.GetRolesByUserId(user.IdUsuario);

            // 4) responder exito
            return new LoginResponse
            {
                Success = true,
                IdUsuario = user.IdUsuario,
                Username = user.Username,
                Activo = user.Activo,
                Roles = roles.ToArray(),
                Message = "Login correcto"
            };
        }

        
    }
}