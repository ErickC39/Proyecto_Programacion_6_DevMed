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

            // 2) bloqueo por intentos fallidos (fuerza bruta)
            if (user.BloqueadoHasta.HasValue && user.BloqueadoHasta.Value > DateTime.Now)
            {
                var minutosRestantes = Math.Max(1, (int)Math.Ceiling((user.BloqueadoHasta.Value - DateTime.Now).TotalMinutes));
                return new LoginResponse
                {
                    Success = false,
                    Message = $"Cuenta bloqueada temporalmente por demasiados intentos fallidos. Intente de nuevo en {minutosRestantes} minuto(s)."
                };
            }

            // 3) validar password contra hash + salt
            bool ok = PasswordVerifier.VerifySha512(user.PasswordSalt, user.PasswordHash, password);
            if (!ok)
            {
                repo.RegistrarIntentoFallido(user.IdUsuario);
                return new LoginResponse { Success = false, Message = "Usuario o contrasenia incorrectos." };
            }

            repo.RegistrarLoginExitoso(user.IdUsuario);

            // 4) traer rol(es)
            var roles = repo.GetRolesByUserId(user.IdUsuario);

            // 5) responder exito
            return new LoginResponse
            {
                Success = true,
                IdUsuario = user.IdUsuario,
                Username = user.Username,
                Nombre = user.Nombre,
                Activo = user.Activo,
                Roles = roles.ToArray(),
                Message = "Login correcto"
            };
        }

        
    }
}