using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    public class VisitanteRepository
    {
        private readonly string _connectionString;

        public VisitanteRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        public List<VisitanteDto> Listar()
        {
            var lista = new List<VisitanteDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Visitantes ORDER BY FechaHoraEntrada DESC;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Map(reader));
            return lista;
        }

        public VisitanteDto? ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Visitantes WHERE IdVisita = @Id;";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return Map(reader);
        }

        public RespuestaCrud Crear(VisitanteDto m)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Visitante_Crear";
            cmd.Parameters.Add(new SqlParameter("@NombreCompleto", SqlDbType.NVarChar, 150) { Value = m.NombreCompleto });
            cmd.Parameters.Add(new SqlParameter("@IdPacienteAVisitar", SqlDbType.Int) { Value = m.IdPacienteAVisitar });
            cmd.Parameters.Add(new SqlParameter("@FechaHoraEntrada", SqlDbType.DateTime) { Value = m.FechaHoraEntrada });
            var pId = new SqlParameter("@IdGenerado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pId);
            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Entrada registrada correctamente.", IdGenerado = pId.Value == DBNull.Value ? 0 : (int)pId.Value };
        }

        public RespuestaCrud RegistrarSalida(int idVisita, DateTime? fechaHoraSalida)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Visitante_RegistrarSalida";
            cmd.Parameters.Add(new SqlParameter("@IdVisita", SqlDbType.Int) { Value = idVisita });
            cmd.Parameters.Add(new SqlParameter("@FechaHoraSalida", SqlDbType.DateTime) { Value = (object?)fechaHoraSalida ?? DBNull.Value });
            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Salida registrada correctamente." };
        }

        public RespuestaCrud Actualizar(VisitanteDto m)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Visitante_Actualizar";
            cmd.Parameters.Add(new SqlParameter("@IdVisita", SqlDbType.Int) { Value = m.IdVisita });
            cmd.Parameters.Add(new SqlParameter("@Identificacion", SqlDbType.NVarChar, 50) { Value = m.Identificacion });
            cmd.Parameters.Add(new SqlParameter("@NombreCompleto", SqlDbType.NVarChar, 150) { Value = m.NombreCompleto });
            cmd.Parameters.Add(new SqlParameter("@IdPacienteAVisitar", SqlDbType.Int) { Value = m.IdPacienteAVisitar });
            cmd.Parameters.Add(new SqlParameter("@FechaHoraEntrada", SqlDbType.DateTime) { Value = m.FechaHoraEntrada });
            cmd.Parameters.Add(new SqlParameter("@FechaHoraSalida", SqlDbType.DateTime) { Value = (object?)m.FechaHoraSalida ?? DBNull.Value });
            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Registro actualizado correctamente." };
        }

        public RespuestaCrud Eliminar(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Visitante_Eliminar";
            cmd.Parameters.Add(new SqlParameter("@IdVisita", SqlDbType.Int) { Value = id });
            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Registro eliminado correctamente." };
        }

        private static VisitanteDto Map(SqlDataReader r)
        {
            return new VisitanteDto
            {
                IdVisita = r.GetInt32(r.GetOrdinal("IdVisita")),
                Identificacion = r.GetString(r.GetOrdinal("Identificacion")),
                NombreCompleto = r.GetString(r.GetOrdinal("NombreCompleto")),
                IdPacienteAVisitar = r.GetInt32(r.GetOrdinal("IdPacienteAVisitar")),
                PacienteVisitado = r["PacienteVisitado"] as string,
                FechaHoraEntrada = r.GetDateTime(r.GetOrdinal("FechaHoraEntrada")),
                FechaHoraSalida = r["FechaHoraSalida"] == DBNull.Value ? null : r.GetDateTime(r.GetOrdinal("FechaHoraSalida")),
                MinutosEstancia = r["MinutosEstancia"] == DBNull.Value ? null : r.GetInt32(r.GetOrdinal("MinutosEstancia"))
            };
        }
    }
}
