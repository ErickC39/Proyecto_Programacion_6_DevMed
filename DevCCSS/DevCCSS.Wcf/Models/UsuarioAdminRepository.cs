using DevCCSS.Wcf.Contracts;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    // Repositorio para el CRUD de usuarios (distinto del UsuarioRepository del login).
    public class UsuarioAdminRepository
    {
        private readonly string _connectionString;

        public UsuarioAdminRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        public List<UsuarioDto> Listar()
        {
            var lista = new List<UsuarioDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Usuarios ORDER BY Username;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read()) lista.Add(Map(reader));
            return lista;
        }

        public UsuarioDto? ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Usuarios WHERE IdUsuario = @Id;";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return Map(reader);
        }

        public RespuestaCrud Crear(UsuarioDto u)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Usuario_Crear";
            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = u.Nombre });
            cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = u.Username });
            cmd.Parameters.Add(new SqlParameter("@Password", SqlDbType.NVarChar, 100) { Value = (object?)u.Password ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IdRol", SqlDbType.Int) { Value = u.IdRol });
            cmd.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = u.Activo });
            var pId = new SqlParameter("@IdGenerado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pId);
            conn.Open();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Usuario creado correctamente.", IdGenerado = pId.Value == DBNull.Value ? 0 : (int)pId.Value };
        }

        public RespuestaCrud Actualizar(UsuarioDto u)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Usuario_Actualizar";
            cmd.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.Int) { Value = u.IdUsuario });
            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = u.Nombre });
            cmd.Parameters.Add(new SqlParameter("@Username", SqlDbType.NVarChar, 50) { Value = u.Username });
            cmd.Parameters.Add(new SqlParameter("@IdRol", SqlDbType.Int) { Value = u.IdRol });
            cmd.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = u.Activo });
            conn.Open();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Usuario actualizado correctamente." };
        }

        public RespuestaCrud Eliminar(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Usuario_Eliminar";
            cmd.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.Int) { Value = id });
            conn.Open();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Usuario eliminado correctamente." };
        }

        public List<RolDto> ListarRoles()
        {
            var lista = new List<RolDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT IdRol, NombreRol FROM dbo.Roles ORDER BY NombreRol;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(new RolDto { IdRol = reader.GetInt32(0), NombreRol = reader.GetString(1) });
            return lista;
        }

        private static UsuarioDto Map(SqlDataReader r)
        {
            return new UsuarioDto
            {
                IdUsuario = r.GetInt32(r.GetOrdinal("IdUsuario")),
                Nombre = r.GetString(r.GetOrdinal("Nombre")),
                Username = r.GetString(r.GetOrdinal("Username")),
                IdRol = r.GetInt32(r.GetOrdinal("IdRol")),
                Rol = r["Rol"] as string,
                Activo = r.GetBoolean(r.GetOrdinal("Activo")),
                FechaCreacion = r["FechaCreacion"] == DBNull.Value ? null : r.GetDateTime(r.GetOrdinal("FechaCreacion"))
            };
        }
    }
}
