using DevCCSS.Wcf.Contracts;
using Microsoft.Data.SqlClient;

namespace DevCCSS.Wcf.Models
{
    public class RolPermisoRepository
    {
        private readonly string _connectionString;

        public RolPermisoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        public List<RolPermisoDto> Listar()
        {
            var lista = new List<RolPermisoDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_RolesPermisos ORDER BY NombreRol, Modulo;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new RolPermisoDto
                {
                    IdRolPermiso = reader.GetInt32(reader.GetOrdinal("IdRolPermiso")),
                    IdRol = reader.GetInt32(reader.GetOrdinal("IdRol")),
                    NombreRol = reader.GetString(reader.GetOrdinal("NombreRol")),
                    Modulo = reader.GetString(reader.GetOrdinal("Modulo")),
                    PuedeVer = reader.GetBoolean(reader.GetOrdinal("PuedeVer")),
                    PuedeCrear = reader.GetBoolean(reader.GetOrdinal("PuedeCrear")),
                    PuedeEditar = reader.GetBoolean(reader.GetOrdinal("PuedeEditar")),
                    PuedeEliminar = reader.GetBoolean(reader.GetOrdinal("PuedeEliminar"))
                });
            }
            return lista;
        }
    }
}
