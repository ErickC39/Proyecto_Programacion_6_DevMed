using DevCCSS.Wcf.Contracts;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    public class ExamenMedicoRepository
    {
        private readonly string _connectionString;

        public ExamenMedicoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        // LISTAR usa la VIEW vw_ExamenesMedicos
        public List<ExamenMedicoDto> Listar()
        {
            var lista = new List<ExamenMedicoDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_ExamenesMedicos ORDER BY FechaSolicitud DESC;";

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Map(reader));

            return lista;
        }

        public List<ExamenMedicoDto> ListarPorPaciente(int idPaciente)
        {
            var lista = new List<ExamenMedicoDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_ExamenesMedicos WHERE IdPaciente = @IdPaciente ORDER BY FechaSolicitud DESC;";
            cmd.Parameters.Add(new SqlParameter("@IdPaciente", SqlDbType.Int) { Value = idPaciente });

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Map(reader));

            return lista;
        }

        public ExamenMedicoDto? ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_ExamenesMedicos WHERE IdExamenMedico = @Id;";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return Map(reader);
        }

        public List<TipoExamenDto> ListarTiposExamen()
        {
            var lista = new List<TipoExamenDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT IdTipoExamen, Descripcion FROM dbo.Tipos_Examen WHERE Activo = 1 ORDER BY Descripcion;";

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new TipoExamenDto
                {
                    IdTipoExamen = reader.GetInt32(reader.GetOrdinal("IdTipoExamen")),
                    Descripcion = reader.GetString(reader.GetOrdinal("Descripcion"))
                });
            }

            return lista;
        }

        public List<EstadoExamenDto> ListarEstadosExamen()
        {
            var lista = new List<EstadoExamenDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT IdEstadoExamen, Descripcion FROM dbo.Estados_Examen ORDER BY IdEstadoExamen;";

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new EstadoExamenDto
                {
                    IdEstadoExamen = reader.GetInt32(reader.GetOrdinal("IdEstadoExamen")),
                    Descripcion = reader.GetString(reader.GetOrdinal("Descripcion"))
                });
            }

            return lista;
        }

        public List<PacienteExamenDto> ListarPacientes()
        {
            var lista = new List<PacienteExamenDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT IdPaciente, Identificacion, Nombre + ' ' + Apellidos AS NombreCompleto
                FROM dbo.Pacientes
                ORDER BY Apellidos, Nombre;
                """;

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new PacienteExamenDto
                {
                    IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                    Identificacion = reader.GetString(reader.GetOrdinal("Identificacion")),
                    NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto"))
                });
            }

            return lista;
        }

        public List<MedicoExamenDto> ListarMedicos()
        {
            var lista = new List<MedicoExamenDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT e.IdEmpleado, e.Identificacion, e.Nombre + ' ' + e.Apellidos AS NombreCompleto,
                       e.Especialidad
                FROM dbo.Empleados e
                INNER JOIN dbo.Usuarios u ON u.IdUsuario = e.IdUsuario
                INNER JOIN dbo.Roles r ON r.IdRol = u.IdRol
                WHERE r.NombreRol = 'Medico' AND u.Activo = 1
                ORDER BY e.Apellidos, e.Nombre;
                """;

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new MedicoExamenDto
                {
                    IdEmpleado = reader.GetInt32(reader.GetOrdinal("IdEmpleado")),
                    Identificacion = reader.GetString(reader.GetOrdinal("Identificacion")),
                    NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto")),
                    Especialidad = reader["Especialidad"] as string
                });
            }

            return lista;
        }

        public RespuestaCrud Crear(ExamenMedicoDto examen)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_ExamenMedico_Crear";
            AgregarParametrosComunes(cmd, examen, incluirEstado: false);

            var pId = new SqlParameter("@IdGenerado", SqlDbType.Int)
            {
                Direction = ParameterDirection.Output
            };
            cmd.Parameters.Add(pId);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud
            {
                Ok = true,
                Mensaje = "Examen medico solicitado correctamente.",
                IdGenerado = pId.Value == DBNull.Value ? 0 : (int)pId.Value
            };
        }

        public RespuestaCrud Actualizar(ExamenMedicoDto examen)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_ExamenMedico_Actualizar";
            cmd.Parameters.Add(new SqlParameter("@IdExamenMedico", SqlDbType.Int) { Value = examen.IdExamenMedico });
            AgregarParametrosComunes(cmd, examen, incluirEstado: true);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud
            {
                Ok = true,
                Mensaje = "Examen medico actualizado correctamente."
            };
        }

        public RespuestaCrud Eliminar(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_ExamenMedico_Eliminar";
            cmd.Parameters.Add(new SqlParameter("@IdExamenMedico", SqlDbType.Int) { Value = id });

            conn.Open();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud
            {
                Ok = true,
                Mensaje = "Examen medico eliminado correctamente."
            };
        }

        private static void AgregarParametrosComunes(
            SqlCommand cmd,
            ExamenMedicoDto examen,
            bool incluirEstado)
        {
            cmd.Parameters.Add(new SqlParameter("@IdPaciente", SqlDbType.Int) { Value = examen.IdPaciente });
            cmd.Parameters.Add(new SqlParameter("@IdEmpleadoMedico", SqlDbType.Int) { Value = examen.IdEmpleadoMedico });
            cmd.Parameters.Add(new SqlParameter("@IdTipoExamen", SqlDbType.Int) { Value = examen.IdTipoExamen });
            cmd.Parameters.Add(new SqlParameter("@FechaSolicitud", SqlDbType.DateTime2) { Value = examen.FechaSolicitud });
            cmd.Parameters.Add(new SqlParameter("@Observaciones", SqlDbType.NVarChar, 1000)
            {
                Value = (object?)examen.Observaciones ?? DBNull.Value
            });

            if (!incluirEstado) return;

            cmd.Parameters.Add(new SqlParameter("@IdEstadoExamen", SqlDbType.Int) { Value = examen.IdEstadoExamen });
            cmd.Parameters.Add(new SqlParameter("@Resultado", SqlDbType.NVarChar)
            {
                Value = (object?)examen.Resultado ?? DBNull.Value
            });
        }

        private static ExamenMedicoDto Map(SqlDataReader r)
        {
            return new ExamenMedicoDto
            {
                IdExamenMedico = r.GetInt32(r.GetOrdinal("IdExamenMedico")),
                IdPaciente = r.GetInt32(r.GetOrdinal("IdPaciente")),
                PacienteIdentificacion = r.GetString(r.GetOrdinal("PacienteIdentificacion")),
                PacienteNombreCompleto = r.GetString(r.GetOrdinal("PacienteNombreCompleto")),
                IdEmpleadoMedico = r.GetInt32(r.GetOrdinal("IdEmpleadoMedico")),
                MedicoNombreCompleto = r.GetString(r.GetOrdinal("MedicoNombreCompleto")),
                IdTipoExamen = r.GetInt32(r.GetOrdinal("IdTipoExamen")),
                TipoExamen = r.GetString(r.GetOrdinal("TipoExamen")),
                FechaSolicitud = r.GetDateTime(r.GetOrdinal("FechaSolicitud")),
                IdEstadoExamen = r.GetInt32(r.GetOrdinal("IdEstadoExamen")),
                EstadoExamen = r.GetString(r.GetOrdinal("EstadoExamen")),
                Resultado = r["Resultado"] as string,
                Observaciones = r["Observaciones"] as string,
                FechaResultado = r["FechaResultado"] == DBNull.Value
                    ? null
                    : r.GetDateTime(r.GetOrdinal("FechaResultado"))
            };
        }
    }
}
