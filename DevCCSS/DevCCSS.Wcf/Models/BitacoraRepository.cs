using DevCCSS.Wcf.Contracts;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    public class BitacoraRepository
    {
        private readonly string _connectionString;

        public BitacoraRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        public List<BitacoraAuditoriaDto> ListarAuditoria(int top)
        {
            var lista = new List<BitacoraAuditoriaDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT TOP (@Top) * FROM dbo.vw_BitacoraAuditoria ORDER BY Fecha DESC;";
            cmd.Parameters.Add(new SqlParameter("@Top", SqlDbType.Int) { Value = top });
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new BitacoraAuditoriaDto
                {
                    IdAuditoria = reader.GetInt32(reader.GetOrdinal("IdAuditoria")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                    IdUsuario = reader["IdUsuario"] == DBNull.Value ? null : reader.GetInt32(reader.GetOrdinal("IdUsuario")),
                    UsuarioResponsable = reader["UsuarioResponsable"] as string,
                    Accion = reader.GetString(reader.GetOrdinal("Accion")),
                    TablaAfectada = reader.GetString(reader.GetOrdinal("TablaAfectada")),
                    DetalleRegistroAntiguo = reader["DetalleRegistroAntiguo"] as string,
                    DetalleRegistroNuevo = reader["DetalleRegistroNuevo"] as string,
                    DireccionIP = reader["DireccionIP"] as string
                });
            }
            return lista;
        }

        public List<BitacoraErrorDto> ListarErrores(int top)
        {
            var lista = new List<BitacoraErrorDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT TOP (@Top) * FROM dbo.vw_BitacoraErrores ORDER BY Fecha DESC;";
            cmd.Parameters.Add(new SqlParameter("@Top", SqlDbType.Int) { Value = top });
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new BitacoraErrorDto
                {
                    IdError = reader.GetInt32(reader.GetOrdinal("IdError")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                    Procedimiento_Trigger = reader.GetString(reader.GetOrdinal("Procedimiento_Trigger")),
                    NumeroError = reader["NumeroError"] == DBNull.Value ? null : reader.GetInt32(reader.GetOrdinal("NumeroError")),
                    MensajeError = reader.GetString(reader.GetOrdinal("MensajeError")),
                    LineaError = reader["LineaError"] == DBNull.Value ? null : reader.GetInt32(reader.GetOrdinal("LineaError"))
                });
            }
            return lista;
        }

      
    }
}
