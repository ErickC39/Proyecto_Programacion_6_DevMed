using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    public class HabitacionRepository
    {
        private readonly string _connectionString;

        public HabitacionRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        public List<HabitacionDto> Listar()
        {
            var lista = new List<HabitacionDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Habitaciones ORDER BY NumeroHabitacion;";

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Map(reader));

            return lista;
        }

        public HabitacionDto? ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Habitaciones WHERE IdHabitacion = @Id;";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return Map(reader);
        }

        public List<TipoHabitacionDto> ListarTiposHabitacion()
        {
            var lista = new List<TipoHabitacionDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT IdTipoHabitacion, Descripcion, Capacidad FROM dbo.Tipos_Habitacion WHERE Activo = 1 ORDER BY Descripcion;";

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new TipoHabitacionDto
                {
                    IdTipoHabitacion = reader.GetInt32(reader.GetOrdinal("IdTipoHabitacion")),
                    Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                    Capacidad = reader.GetInt32(reader.GetOrdinal("Capacidad"))
                });
            }

            return lista;
        }

        public List<EstadoHabitacionDto> ListarEstadosHabitacion()
        {
            var lista = new List<EstadoHabitacionDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT IdEstadoHabitacion, Descripcion FROM dbo.Estados_Habitacion ORDER BY IdEstadoHabitacion;";

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new EstadoHabitacionDto
                {
                    IdEstadoHabitacion = reader.GetInt32(reader.GetOrdinal("IdEstadoHabitacion")),
                    Descripcion = reader.GetString(reader.GetOrdinal("Descripcion"))
                });
            }

            return lista;
        }

        public List<PacienteHabitacionDto> ListarPacientes()
        {
            var lista = new List<PacienteHabitacionDto>();

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
                lista.Add(new PacienteHabitacionDto
                {
                    IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                    Identificacion = reader.GetString(reader.GetOrdinal("Identificacion")),
                    NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto"))
                });
            }

            return lista;
        }

        public List<EmpleadoHabitacionDto> ListarEmpleados()
        {
            var lista = new List<EmpleadoHabitacionDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT e.IdEmpleado, e.Nombre + ' ' + e.Apellidos AS NombreCompleto
                FROM dbo.Empleados e
                INNER JOIN dbo.Usuarios u ON u.IdUsuario = e.IdUsuario
                WHERE u.Activo = 1
                ORDER BY e.Apellidos, e.Nombre;
                """;

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new EmpleadoHabitacionDto
                {
                    IdEmpleado = reader.GetInt32(reader.GetOrdinal("IdEmpleado")),
                    NombreCompleto = reader.GetString(reader.GetOrdinal("NombreCompleto"))
                });
            }

            return lista;
        }

        public RespuestaCrud Crear(HabitacionDto habitacion)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Habitacion_Crear";
            cmd.Parameters.Add(new SqlParameter("@NumeroHabitacion", SqlDbType.NVarChar, 10) { Value = habitacion.NumeroHabitacion });
            cmd.Parameters.Add(new SqlParameter("@IdTipoHabitacion", SqlDbType.Int) { Value = habitacion.IdTipoHabitacion });
            var pId = new SqlParameter("@IdGenerado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pId);

            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud
            {
                Ok = true,
                Mensaje = "Habitacion registrada correctamente.",
                IdGenerado = pId.Value == DBNull.Value ? 0 : (int)pId.Value
            };
        }

        public RespuestaCrud Actualizar(HabitacionDto habitacion)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Habitacion_Actualizar";
            cmd.Parameters.Add(new SqlParameter("@IdHabitacion", SqlDbType.Int) { Value = habitacion.IdHabitacion });
            cmd.Parameters.Add(new SqlParameter("@NumeroHabitacion", SqlDbType.NVarChar, 10) { Value = habitacion.NumeroHabitacion });
            cmd.Parameters.Add(new SqlParameter("@IdTipoHabitacion", SqlDbType.Int) { Value = habitacion.IdTipoHabitacion });

            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud { Ok = true, Mensaje = "Habitacion actualizada correctamente." };
        }

        public RespuestaCrud Asignar(AsignarHabitacionDto asignacion)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Habitacion_Asignar";
            cmd.Parameters.Add(new SqlParameter("@IdHabitacion", SqlDbType.Int) { Value = asignacion.IdHabitacion });
            cmd.Parameters.Add(new SqlParameter("@IdPaciente", SqlDbType.Int) { Value = asignacion.IdPaciente });
            cmd.Parameters.Add(new SqlParameter("@FechaIngreso", SqlDbType.DateTime2) { Value = asignacion.FechaIngreso });
            cmd.Parameters.Add(new SqlParameter("@IdEmpleadoResponsable", SqlDbType.Int) { Value = asignacion.IdEmpleadoResponsable });

            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud { Ok = true, Mensaje = "Paciente asignado a la habitacion correctamente." };
        }

        public RespuestaCrud Liberar(LiberarHabitacionDto liberacion)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Habitacion_Liberar";
            cmd.Parameters.Add(new SqlParameter("@IdHabitacion", SqlDbType.Int) { Value = liberacion.IdHabitacion });
            cmd.Parameters.Add(new SqlParameter("@IdPaciente", SqlDbType.Int) { Value = liberacion.IdPaciente });
            cmd.Parameters.Add(new SqlParameter("@FechaSalida", SqlDbType.DateTime2) { Value = liberacion.FechaSalida });
            cmd.Parameters.Add(new SqlParameter("@IdEmpleadoResponsable", SqlDbType.Int) { Value = liberacion.IdEmpleadoResponsable });

            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud { Ok = true, Mensaje = "Habitacion liberada correctamente." };
        }

        public RespuestaCrud Eliminar(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Habitacion_Eliminar";
            cmd.Parameters.Add(new SqlParameter("@IdHabitacion", SqlDbType.Int) { Value = id });

            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud { Ok = true, Mensaje = "Habitacion eliminada correctamente." };
        }

        public List<OcupanteHabitacionDto> ListarOcupantesActivos(int idHabitacion)
        {
            var lista = new List<OcupanteHabitacionDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Habitacion_ListarOcupantesActivos";
            cmd.Parameters.Add(new SqlParameter("@IdHabitacion", SqlDbType.Int) { Value = idHabitacion });

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new OcupanteHabitacionDto
                {
                    IdOcupante = reader.GetInt32(reader.GetOrdinal("IdOcupante")),
                    IdHabitacion = reader.GetInt32(reader.GetOrdinal("IdHabitacion")),
                    NumeroHabitacion = reader.GetString(reader.GetOrdinal("NumeroHabitacion")),
                    IdPaciente = reader.GetInt32(reader.GetOrdinal("IdPaciente")),
                    PacienteIdentificacion = reader.GetString(reader.GetOrdinal("PacienteIdentificacion")),
                    PacienteNombreCompleto = reader.GetString(reader.GetOrdinal("PacienteNombreCompleto")),
                    FechaIngreso = reader.GetDateTime(reader.GetOrdinal("FechaIngreso")),
                    FechaSalida = reader["FechaSalida"] == DBNull.Value ? null : reader.GetDateTime(reader.GetOrdinal("FechaSalida")),
                    ResponsableIngreso = reader.GetString(reader.GetOrdinal("ResponsableIngreso")),
                    ResponsableSalida = reader["ResponsableSalida"] as string
                });
            }

            return lista;
        }

        private static HabitacionDto Map(SqlDataReader r)
        {
            return new HabitacionDto
            {
                IdHabitacion = r.GetInt32(r.GetOrdinal("IdHabitacion")),
                NumeroHabitacion = r.GetString(r.GetOrdinal("NumeroHabitacion")),
                IdTipoHabitacion = r.GetInt32(r.GetOrdinal("IdTipoHabitacion")),
                TipoHabitacion = r.GetString(r.GetOrdinal("TipoHabitacion")),
                Capacidad = r.GetInt32(r.GetOrdinal("Capacidad")),
                IdEstadoHabitacion = r.GetInt32(r.GetOrdinal("IdEstadoHabitacion")),
                EstadoHabitacion = r.GetString(r.GetOrdinal("EstadoHabitacion")),
                OcupantesActuales = r.GetInt32(r.GetOrdinal("OcupantesActuales"))
            };
        }
    }
}
