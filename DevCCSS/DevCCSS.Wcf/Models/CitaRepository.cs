using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    public class CitaRepository
    {
        private readonly string _connectionString;

        public CitaRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontró DefaultConnection en appsettings.json");
        }

        public List<CitaDto> Listar()
        {
            var lista = new List<CitaDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Citas ORDER BY FechaHoraCita DESC;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Map(reader));
            return lista;
        }

        public CitaDto? ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Citas WHERE IdCita = @Id;";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return Map(reader);
        }

        public RespuestaCita Agendar(AgendarCitaDto dto)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Cita_Agendar";

            cmd.Parameters.Add(new SqlParameter("@Identificacion", SqlDbType.NVarChar, 50) { Value = dto.PacienteIdentificacion });
            cmd.Parameters.Add(new SqlParameter("@IdMedico", SqlDbType.Int) { Value = dto.IdMedico });
            cmd.Parameters.Add(new SqlParameter("@FechaHoraCita", SqlDbType.DateTime) { Value = dto.FechaHoraCita });
            cmd.Parameters.Add(new SqlParameter("@PrioridadCita", SqlDbType.NVarChar, 20) { Value = string.IsNullOrEmpty(dto.PrioridadCita) ? "Normal" : dto.PrioridadCita });

            var pId = new SqlParameter("@IdCitaGenerada", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var pMsg = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
            var pCode = new SqlParameter("@CodigoSalida", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var pHora = new SqlParameter("@HoraSugerida", SqlDbType.NVarChar, 10) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pId);
            cmd.Parameters.Add(pMsg);
            cmd.Parameters.Add(pCode);
            cmd.Parameters.Add(pHora);

            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();

            int codigo = pCode.Value == DBNull.Value ? -1 : (int)pCode.Value;

            return new RespuestaCita
            {
                Ok = codigo == 0,
                Mensaje = pMsg.Value?.ToString() ?? string.Empty,
                IdGenerado = pId.Value == DBNull.Value ? 0 : (int)pId.Value,
                CodigoDisponibilidad = codigo,
                HoraSugerida = pHora.Value == DBNull.Value ? null : pHora.Value.ToString()
            };
        }

        public RespuestaCita AgendarEmergencia(AgendarEmergenciaDto dto)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Cita_AgendarEmergencia";

            cmd.Parameters.Add(new SqlParameter("@IdEmpleado", SqlDbType.Int) { Value = dto.IdEmpleado });
            cmd.Parameters.Add(new SqlParameter("@IdMedico", SqlDbType.Int) { Value = dto.IdMedico });
            cmd.Parameters.Add(new SqlParameter("@FechaHoraCita", SqlDbType.DateTime) { Value = dto.FechaHoraCita });

            var pId = new SqlParameter("@IdCitaGenerada", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var pReagendadas = new SqlParameter("@CitasReagendadas", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var pMsg = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
            var pCode = new SqlParameter("@CodigoSalida", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pId);
            cmd.Parameters.Add(pReagendadas);
            cmd.Parameters.Add(pMsg);
            cmd.Parameters.Add(pCode);

            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();

            int codigo = pCode.Value == DBNull.Value ? -1 : (int)pCode.Value;

            return new RespuestaCita
            {
                Ok = codigo == 0,
                Mensaje = pMsg.Value?.ToString() ?? string.Empty,
                IdGenerado = pId.Value == DBNull.Value ? 0 : (int)pId.Value,
                CitasReagendadas = pReagendadas.Value == DBNull.Value ? 0 : (int)pReagendadas.Value,
                CodigoDisponibilidad = codigo
            };
        }

        public RespuestaCita RegistrarLlegada(int idCita)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Cita_RegistrarLlegada";

            cmd.Parameters.Add(new SqlParameter("@IdCita", SqlDbType.Int) { Value = idCita });

            var pMsg = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            var pCode = new SqlParameter("@CodigoSalida", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(pMsg);
            cmd.Parameters.Add(pCode);

            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();

            int codigo = pCode.Value == DBNull.Value ? -1 : (int)pCode.Value;

            return new RespuestaCita
            {
                Ok = codigo == 0,
                Mensaje = pMsg.Value?.ToString() ?? ""
            };
        }

        public RespuestaCita IniciarAtencion(int idCita)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Cita_IniciarAtencion";

            cmd.Parameters.Add(new SqlParameter("@IdCita", SqlDbType.Int) { Value = idCita });

            var pMsg = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500)
            {
                Direction = ParameterDirection.Output
            };

            var pCode = new SqlParameter("@CodigoSalida", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };

            cmd.Parameters.Add(pMsg);
            cmd.Parameters.Add(pCode);

            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();

            int codigo = pCode.Value == DBNull.Value ? -1 : (int)pCode.Value;

            return new RespuestaCita
            {
                Ok = codigo == 0,
                Mensaje = pMsg.Value?.ToString() ?? ""
            };
        }
        public RespuestaCita Finalizar(FinalizarCitaDto dto)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Cita_Finalizar";

            cmd.Parameters.Add(new SqlParameter("@IdCita", SqlDbType.Int) { Value = dto.IdCita });
            cmd.Parameters.Add(new SqlParameter("@ResultadoConsulta", SqlDbType.NVarChar) { Value = dto.ResultadoConsulta });
            cmd.Parameters.Add(new SqlParameter("@RequiereControl", SqlDbType.Bit) { Value = dto.RequiereControl });
            cmd.Parameters.Add(new SqlParameter("@FechaControl", SqlDbType.DateTime) { Value = (object?)dto.FechaControl ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@DetallesControl", SqlDbType.NVarChar) { Value = (object?)dto.DetallesControl ?? DBNull.Value });

            var pIdControl = new SqlParameter("@IdCitaControlGenerada", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var pMsg = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
            var pCode = new SqlParameter("@CodigoSalida", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pIdControl);
            cmd.Parameters.Add(pMsg);
            cmd.Parameters.Add(pCode);

            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();

            string msg = pMsg.Value?.ToString() ?? string.Empty;
            int idControl = pIdControl.Value == DBNull.Value ? 0 : (int)pIdControl.Value;
            int codigo = pCode.Value == DBNull.Value ? -1 : (int)pCode.Value;

            // NOTA: sp_Cita_Finalizar debe agregar el parámetro de salida @CodigoSalida INT
            // (0 = éxito, distinto de 0 = error), igual que los demás procedimientos.
            // Antes esto se determinaba leyendo si el texto del mensaje contenía "finalizada",
            // lo cual se rompe si cambia la redacción del mensaje en el SP.
            return new RespuestaCita { Ok = codigo == 0, Mensaje = msg, IdCitaControl = idControl };
        }

        // CitaRepository.cs (Wcf) — agregar
        public RespuestaCita Cancelar(int idCita)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Cita_Cancelar";
            cmd.Parameters.Add(new SqlParameter("@IdCita", SqlDbType.Int) { Value = idCita });
            var pMsg = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
            var pCode = new SqlParameter("@CodigoSalida", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pMsg); cmd.Parameters.Add(pCode);
            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();
            int codigo = pCode.Value == DBNull.Value ? -1 : (int)pCode.Value;
            return new RespuestaCita { Ok = codigo == 0, Mensaje = pMsg.Value?.ToString() ?? string.Empty };
        }

        public RespuestaCita Eliminar(int idCita)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Cita_Eliminar";
            cmd.Parameters.Add(new SqlParameter("@IdCita", SqlDbType.Int) { Value = idCita });
            var pMsg = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
            var pCode = new SqlParameter("@CodigoSalida", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pMsg); cmd.Parameters.Add(pCode);
            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();
            int codigo = pCode.Value == DBNull.Value ? -1 : (int)pCode.Value;
            return new RespuestaCita { Ok = codigo == 0, Mensaje = pMsg.Value?.ToString() ?? string.Empty };
        }

        private static CitaDto Map(SqlDataReader r) => new CitaDto
        {
            IdCita = r.GetInt32(r.GetOrdinal("IdCita")),
            IdPaciente = r.GetInt32(r.GetOrdinal("IdPaciente")),
            PacienteIdentificacion = r.GetString(r.GetOrdinal("PacienteIdentificacion")),
            PacienteNombreCompleto = r.GetString(r.GetOrdinal("PacienteNombreCompleto")),
            IdEmpleado_Medico = r.GetInt32(r.GetOrdinal("IdEmpleado_Medico")),
            MedicoNombreCompleto = r.GetString(r.GetOrdinal("MedicoNombreCompleto")),
            Especialidad = r["Especialidad"] as string,
            FechaHoraCita = r.GetDateTime(r.GetOrdinal("FechaHoraCita")),
            FechaHoraLlegada = r["FechaHoraLlegada"] == DBNull.Value ? null : r.GetDateTime(r.GetOrdinal("FechaHoraLlegada")),
            TiempoEsperaMinutos = r.GetInt32(r.GetOrdinal("TiempoEsperaMinutos")),
            EstadoCita = r.GetString(r.GetOrdinal("EstadoCita")),
            ResultadoConsulta = r["ResultadoConsulta"] as string,
            RequiereControl = r.GetBoolean(r.GetOrdinal("RequiereControl")),
            PrioridadCita = r["PrioridadCita"] as string,
            EsCitaControl = r["EsCitaControl"] != DBNull.Value && r.GetBoolean(r.GetOrdinal("EsCitaControl")),
            CitaPreviaEsControl = r["CitaPreviaEsControl"] != DBNull.Value && r.GetBoolean(r.GetOrdinal("CitaPreviaEsControl")),
            FueReagendadaPorEmergencia = r["FueReagendadaPorEmergencia"] != DBNull.Value && r.GetBoolean(r.GetOrdinal("FueReagendadaPorEmergencia")),
            MensajeReagendo = r["MensajeReagendo"] as string,
            IdTipoHabitacionRequerido = r["IdTipoHabitacionRequerido"] == DBNull.Value ? null : r.GetInt32(r.GetOrdinal("IdTipoHabitacionRequerido")),
            TipoHabitacionRequerido = r["TipoHabitacionRequerido"] as string,
            IdHabitacionAsignada = r["IdHabitacionAsignada"] == DBNull.Value ? null : r.GetInt32(r.GetOrdinal("IdHabitacionAsignada")),
            NumeroHabitacionAsignada = r["NumeroHabitacionAsignada"] as string,
        };
    }
}