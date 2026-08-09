using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Infrastructure;
using Microsoft.Data.SqlClient;

namespace DevCCSS.Wcf.Models
{
    public class MedicoRepository
    {
        private readonly string _connectionString;

        public MedicoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new Exception("No existe DefaultConnection");
        }

        public List<MedicoDto> Listar()
        {
            var lista = new List<MedicoDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            SELECT
                m.IdMedico,
                e.IdEmpleado,
                e.Identificacion,
                e.Nombre,
                e.Apellidos,
                esp.IdEspecialidad,
                esp.Nombre AS Especialidad,
                e.Activo
            FROM Medicos m
            INNER JOIN Empleados e
                ON e.IdEmpleado = m.IdEmpleado
            LEFT JOIN Especialidades esp
                ON esp.IdEspecialidad = m.IdEspecialidad
            ORDER BY e.Apellidos, e.Nombre";

            conn.Open();

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new MedicoDto
                {
                    IdMedico = reader.GetInt32(0),
                    IdEmpleado = reader.GetInt32(1),
                    Identificacion = reader.GetString(2),
                    Nombre = reader.GetString(3),
                    Apellidos = reader.GetString(4),
                    IdEspecialidad = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    Especialidad = reader.IsDBNull(6) ? "Sin especialidad" : reader.GetString(6),
                    Activo = reader.GetBoolean(7)
                });
            }

            return lista;
        }

        public MedicoDto? ObtenerPorId(int idMedico)
        {
            return Listar()
                .FirstOrDefault(x => x.IdMedico == idMedico);
        }

        public RespuestaCrud Crear(MedicoDto medico)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
    INSERT INTO Medicos
    (
        IdEmpleado,
        IdEspecialidad
    )
    VALUES
    (
        @IdEmpleado,
        @IdEspecialidad
    )";

            cmd.Parameters.AddWithValue("@IdEmpleado", medico.IdEmpleado);
            cmd.Parameters.AddWithValue("@IdEspecialidad", medico.IdEspecialidad);

            conn.Open();
            conn.EstablecerUsuarioAuditoria();

            cmd.ExecuteNonQuery();

            return new RespuestaCrud
            {
                Ok = true,
                Mensaje = "Medico registrado correctamente."
            };
        }

        public RespuestaCrud Actualizar(MedicoDto medico)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
    UPDATE Medicos
    SET IdEspecialidad = @IdEspecialidad
    WHERE IdMedico = @IdMedico";

            cmd.Parameters.AddWithValue("@IdEspecialidad", medico.IdEspecialidad);
            cmd.Parameters.AddWithValue("@IdMedico", medico.IdMedico);

            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud
            {
                Ok = true,
                Mensaje = "Medico actualizado correctamente."
            };
        }

        public RespuestaCrud Eliminar(int idMedico)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();
            conn.EstablecerUsuarioAuditoria();

            using (var cmdHorarios = conn.CreateCommand())
            {
                cmdHorarios.CommandText = "DELETE FROM Horarios_Medicos WHERE IdMedico = @IdMedico";
                cmdHorarios.Parameters.AddWithValue("@IdMedico", idMedico);
                cmdHorarios.ExecuteNonQuery();
            }

            using (var cmdMedico = conn.CreateCommand())
            {
                cmdMedico.CommandText = "DELETE FROM Medicos WHERE IdMedico = @IdMedico";
                cmdMedico.Parameters.AddWithValue("@IdMedico", idMedico);
                cmdMedico.ExecuteNonQuery();
            }

            return new RespuestaCrud
            {
                Ok = true,
                Mensaje = "Medico eliminado correctamente."
            };
        }

        public RespuestaCrud AgregarHorario(HorarioMedicoDto h)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            INSERT INTO Horarios_Medicos
            (
                IdMedico,
                DiaSemana,
                HoraInicio,
                HoraFin
            )
            VALUES
            (
                @IdMedico,
                @DiaSemana,
                @HoraInicio,
                @HoraFin
            )";

            cmd.Parameters.AddWithValue("@IdMedico", h.IdMedico);
            cmd.Parameters.AddWithValue("@DiaSemana", h.DiaSemana);
            cmd.Parameters.AddWithValue("@HoraInicio", h.HoraInicio);
            cmd.Parameters.AddWithValue("@HoraFin", h.HoraFin);

            conn.Open();

            cmd.ExecuteNonQuery();

            return new RespuestaCrud
            {
                Ok = true,
                Mensaje = "Horario agregado correctamente."
            };
        }

        public List<HorarioMedicoDto> ListarHorarios(int idMedico)
        {
            var lista = new List<HorarioMedicoDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            SELECT
                IdHorario,
                IdMedico,
                DiaSemana,
                HoraInicio,
                HoraFin
            FROM Horarios_Medicos
            WHERE IdMedico=@IdMedico";

            cmd.Parameters.AddWithValue("@IdMedico", idMedico);

            conn.Open();

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new HorarioMedicoDto
                {
                    IdHorario = reader.GetInt32(0),
                    IdMedico = reader.GetInt32(1),
                    DiaSemana = reader.GetByte(2),
                    HoraInicio = reader.GetTimeSpan(3),
                    HoraFin = reader.GetTimeSpan(4)
                });
            }

            return lista;
        }

        public List<CitaDto> ListarCitasAsignadas(int idMedico)
        {
            var lista = new List<CitaDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();

            cmd.CommandText = @"
            SELECT
                c.IdCita,
                c.IdPaciente,
                p.Nombre + ' ' + p.Apellidos,
                c.FechaHoraCita,
                c.EstadoCita
            FROM Citas_Medicas c
            INNER JOIN Pacientes p
                ON p.IdPaciente = c.IdPaciente
            INNER JOIN Medicos m
                ON m.IdEmpleado = c.IdEmpleado_Medico
            WHERE m.IdMedico=@IdMedico
            ORDER BY c.FechaHoraCita";

            cmd.Parameters.AddWithValue("@IdMedico", idMedico);

            conn.Open();

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                lista.Add(new CitaDto
                {
                    IdCita = reader.GetInt32(0),
                    IdPaciente = reader.GetInt32(1),
                    PacienteNombreCompleto = reader.GetString(2),
                    FechaHoraCita = reader.GetDateTime(3),
                    EstadoCita = reader.GetString(4)
                });
            }

            return lista;
        }
    }
}
