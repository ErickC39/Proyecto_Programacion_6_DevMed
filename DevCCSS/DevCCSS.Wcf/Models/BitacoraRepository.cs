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

        public List<BitacoraNotificacionDto> ListarNotificaciones(int top)
        {
            var lista = new List<BitacoraNotificacionDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP (@Top)
                    n.IdNotificacion, n.Fecha, n.IdCitaAfectada, n.IdCitaEmergencia,
                    p.Nombre + ' ' + p.Apellidos AS PacienteAfectado,
                    n.Mensaje
                FROM dbo.Bitacora_Notificaciones n
                LEFT JOIN dbo.Citas_Medicas c ON c.IdCita = COALESCE(n.IdCitaAfectada, n.IdCitaEmergencia)
                LEFT JOIN dbo.Pacientes p ON p.IdPaciente = c.IdPaciente
                ORDER BY n.Fecha DESC;";
            cmd.Parameters.Add(new SqlParameter("@Top", SqlDbType.Int) { Value = top });
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new BitacoraNotificacionDto
                {
                    IdNotificacion = reader.GetInt32(reader.GetOrdinal("IdNotificacion")),
                    Fecha = reader.GetDateTime(reader.GetOrdinal("Fecha")),
                    IdCitaAfectada = reader["IdCitaAfectada"] == DBNull.Value ? null : reader.GetInt32(reader.GetOrdinal("IdCitaAfectada")),
                    IdCitaEmergencia = reader["IdCitaEmergencia"] == DBNull.Value ? null : reader.GetInt32(reader.GetOrdinal("IdCitaEmergencia")),
                    PacienteAfectado = reader["PacienteAfectado"] as string,
                    Mensaje = reader.GetString(reader.GetOrdinal("Mensaje"))
                });
            }
            return lista;
        }
    }
}
