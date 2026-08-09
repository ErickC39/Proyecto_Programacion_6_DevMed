using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    public class EmpleadoRepository
    {
        private readonly string _connectionString;

        public EmpleadoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        public List<EmpleadoDto> Listar()
        {
            var lista = new List<EmpleadoDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Empleados ORDER BY Apellidos, Nombre;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Map(reader));
            return lista;
        }

        public EmpleadoDto? ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Empleados WHERE IdEmpleado = @Id;";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return Map(reader);
        }

        public RespuestaCrud Crear(EmpleadoDto m)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Empleado_Crear";
            AgregarParametrosComunes(cmd, m);
            var pId = new SqlParameter("@IdGenerado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pId);
            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Registro creado correctamente.", IdGenerado = pId.Value == DBNull.Value ? 0 : (int)pId.Value };
        }

        public RespuestaCrud Actualizar(EmpleadoDto m)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Empleado_Actualizar";
            cmd.Parameters.Add(new SqlParameter("@IdEmpleado", SqlDbType.Int) { Value = m.IdEmpleado });
            cmd.Parameters.Add(new SqlParameter("@Identificacion", SqlDbType.NVarChar, 50) { Value = m.Identificacion });
            AgregarParametrosComunes(cmd, m);
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
            cmd.CommandText = "dbo.sp_Empleado_Eliminar";
            cmd.Parameters.Add(new SqlParameter("@IdEmpleado", SqlDbType.Int) { Value = id });
            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Registro eliminado correctamente." };
        }

        public RespuestaCrud CambiarEstado(int idEmpleado, bool activo)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Empleado_CambiarEstado";
            cmd.Parameters.Add(new SqlParameter("@IdEmpleado", SqlDbType.Int) { Value = idEmpleado });
            cmd.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = activo });
            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = activo ? "Empleado activado." : "Empleado desactivado." };
        }

        private static void AgregarParametrosComunes(SqlCommand cmd, EmpleadoDto m)
        {
            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = m.Nombre });
            cmd.Parameters.Add(new SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) { Value = m.Apellidos });
            cmd.Parameters.Add(new SqlParameter("@SalarioPorHora", SqlDbType.Decimal) { Value = m.SalarioPorHora });
            cmd.Parameters.Add(new SqlParameter("@IdUsuario", SqlDbType.Int) { Value = (object?)m.IdUsuario ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IdPacienteVinculado", SqlDbType.Int) { Value = (object?)m.IdPacienteVinculado ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Activo", SqlDbType.Bit) { Value = m.Activo });
        }

        private static EmpleadoDto Map(SqlDataReader r)
        {
            return new EmpleadoDto
            {
                IdEmpleado = r.GetInt32(r.GetOrdinal("IdEmpleado")),
                Identificacion = r.GetString(r.GetOrdinal("Identificacion")),
                Nombre = r.GetString(r.GetOrdinal("Nombre")),
                Apellidos = r.GetString(r.GetOrdinal("Apellidos")),
                SalarioPorHora = r.GetDecimal(r.GetOrdinal("SalarioPorHora")),
                IdUsuario = r["IdUsuario"] == DBNull.Value ? null : r.GetInt32(r.GetOrdinal("IdUsuario")),
                UsuarioAsignado = r["UsuarioAsignado"] as string,
                Cargo = r["Cargo"] as string,
                IdPacienteVinculado = r["IdPacienteVinculado"] == DBNull.Value ? null : r.GetInt32(r.GetOrdinal("IdPacienteVinculado")),
                NombrePacienteVinculado = r["NombrePacienteVinculado"] as string,
                Activo = r.GetBoolean(r.GetOrdinal("Activo"))
            };
        }
    }
}
