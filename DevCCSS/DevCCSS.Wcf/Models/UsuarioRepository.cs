using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    // Habla con la tabla Usuarios y Roles para el login.
    public class UsuarioRepository
    {
        private readonly string _connectionString;

        public UsuarioRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        // Busca el usuario por su nombre de usuario.
        public UsuarioRecord? GetByUsername(string username)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
SELECT IdUsuario, Username, Nombre, PasswordHash, PasswordSalt, Activo, IntentosFallidos, BloqueadoHasta
FROM dbo.Usuarios
WHERE Username = @Username;";

            cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = username });

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new UsuarioRecord
            {
                IdUsuario = reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                Username = reader.GetString(reader.GetOrdinal("Username")),
                Nombre = reader["Nombre"] as string ?? string.Empty,
                PasswordHash = (byte[])reader["PasswordHash"],
                PasswordSalt = (byte[])reader["PasswordSalt"],
                Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                IntentosFallidos = reader.GetInt32(reader.GetOrdinal("IntentosFallidos")),
                BloqueadoHasta = reader["BloqueadoHasta"] == DBNull.Value ? null : reader.GetDateTime(reader.GetOrdinal("BloqueadoHasta"))
            };
        }

        // Se llama cuando la contrasenia no coincide; a partir del intento
        // numero @MaxIntentos la cuenta queda bloqueada temporalmente
        // (ver sp_Usuario_RegistrarIntentoFallido).
        public void RegistrarIntentoFallido(int idUsuario)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Usuario_RegistrarIntentoFallido";
            cmd.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.Int) { Value = idUsuario });
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // Se llama tras un login correcto para reiniciar el contador.
        public void RegistrarLoginExitoso(int idUsuario)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Usuario_RegistrarLoginExitoso";
            cmd.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.Int) { Value = idUsuario });
            conn.Open();
            cmd.ExecuteNonQuery();
        }

        // En este modelo cada usuario tiene UN rol (IdRol en la tabla Usuarios).
        // Devolvemos una lista igual para mantener el formato Roles[] del login.
        public List<string> GetRolesByUserId(int idUsuario)
        {
            var roles = new List<string>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
SELECT r.NombreRol
FROM dbo.Usuarios u
JOIN dbo.Roles r ON r.IdRol = u.IdRol
WHERE u.IdUsuario = @IdUsuario;";

            cmd.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.Int) { Value = idUsuario });

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                roles.Add(reader.GetString(0));

            return roles;
        }
    }
}
