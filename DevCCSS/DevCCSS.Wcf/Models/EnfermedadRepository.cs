using DevCCSS.Wcf.Contracts;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    public class EnfermedadRepository
    {
        private readonly string _connectionString;

        public EnfermedadRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        public List<EnfermedadDto> Listar()
        {
            var lista = new List<EnfermedadDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Enfermedades ORDER BY Nombre;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Map(reader));
            return lista;
        }

        public EnfermedadDto? ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Enfermedades WHERE IdEnfermedad = @Id;";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return Map(reader);
        }

        public RespuestaCrud Crear(EnfermedadDto m)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Enfermedad_Crear";
            AgregarParametros(cmd, m, incluirId: false);
            var pId = new SqlParameter("@IdGenerado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pId);
            conn.Open();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Registro creado correctamente.", IdGenerado = pId.Value == DBNull.Value ? 0 : (int)pId.Value };
        }

        public RespuestaCrud Actualizar(EnfermedadDto m)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Enfermedad_Actualizar";
            AgregarParametros(cmd, m, incluirId: true);
            conn.Open();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Registro actualizado correctamente." };
        }

        public RespuestaCrud Eliminar(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Enfermedad_Eliminar";
            cmd.Parameters.Add(new SqlParameter("@IdEnfermedad", SqlDbType.Int) { Value = id });
            conn.Open();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Registro eliminado correctamente." };
        }

        private static void AgregarParametros(SqlCommand cmd, EnfermedadDto m, bool incluirId)
        {
            if (incluirId)
                cmd.Parameters.Add(new SqlParameter("@IdEnfermedad", SqlDbType.Int) { Value = m.IdEnfermedad });
            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 150) { Value = m.Nombre });
            cmd.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar) { Value = (object?)m.Descripcion ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@RecomendacionesGenerales", SqlDbType.NVarChar) { Value = (object?)m.RecomendacionesGenerales ?? DBNull.Value });
        }

        private static EnfermedadDto Map(SqlDataReader r)
        {
            return new EnfermedadDto
            {
                IdEnfermedad = r.GetInt32(r.GetOrdinal("IdEnfermedad")),
                Nombre = r.GetString(r.GetOrdinal("Nombre")),
                Descripcion = r["Descripcion"] as string,
                RecomendacionesGenerales = r["RecomendacionesGenerales"] as string
            };
        }
    }
}
