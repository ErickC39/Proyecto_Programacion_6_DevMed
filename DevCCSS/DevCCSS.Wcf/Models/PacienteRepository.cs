using DevCCSS.Wcf.Contracts;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
   
    public class PacienteRepository
    {
        private readonly string _connectionString;

        public PacienteRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        // LISTAR usa la VIEW vw_Pacientes
        public List<PacienteDto> Listar()
        {
            var lista = new List<PacienteDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Pacientes ORDER BY Apellidos, Nombre;";

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Map(reader));

            return lista;
        }

        //  OBTENER POR ID
        public PacienteDto? ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Pacientes WHERE IdPaciente = @Id;";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return Map(reader);
        }

        // CREAR Stored Procedure con rollback
        public RespuestaCrud Crear(PacienteDto p)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Paciente_Crear";
            AgregarParametros(cmd, p, incluirId: false);

            // parametro de salida con el Id que se genero
            var pId = new SqlParameter("@IdGenerado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pId);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud
            {
                Ok = true,
                Mensaje = "Paciente creado correctamente.",
                IdGenerado = pId.Value == DBNull.Value ? 0 : (int)pId.Value
            };
        }

        //  ACTUALIZAR Stored Procedure con rollback
        public RespuestaCrud Actualizar(PacienteDto p)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Paciente_Actualizar";
            AgregarParametros(cmd, p, incluirId: true);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud { Ok = true, Mensaje = "Paciente actualizado correctamente." };
        }

        // ELIMINAR Stored Procedure con rollback
        public RespuestaCrud Eliminar(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Paciente_Eliminar";
            cmd.Parameters.Add(new SqlParameter("@IdPaciente", SqlDbType.Int) { Value = id });

            conn.Open();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud { Ok = true, Mensaje = "Paciente eliminado correctamente." };
        }

        // helpers
        private static void AgregarParametros(SqlCommand cmd, PacienteDto p, bool incluirId)
        {
            if (incluirId)
                cmd.Parameters.Add(new SqlParameter("@IdPaciente", SqlDbType.Int) { Value = p.IdPaciente });

            cmd.Parameters.Add(new SqlParameter("@Identificacion", SqlDbType.NVarChar, 50) { Value = p.Identificacion });
            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = p.Nombre });
            cmd.Parameters.Add(new SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) { Value = p.Apellidos });
            cmd.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = p.FechaNacimiento });
            cmd.Parameters.Add(new SqlParameter("@AntecedentesMedicos", SqlDbType.NVarChar) { Value = (object?)p.AntecedentesMedicos ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@EsRecienNacido", SqlDbType.Bit) { Value = p.EsRecienNacido });
            cmd.Parameters.Add(new SqlParameter("@IdSexoBiologico", SqlDbType.Int) { Value = (object?)p.IdSexoBiologico ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IdIdentidadGenero", SqlDbType.Int) { Value = (object?)p.IdIdentidadGenero ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IdTipoSangre", SqlDbType.Int) { Value = (object?)p.IdTipoSangre ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Peso", SqlDbType.Decimal) { Value = (object?)p.Peso ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Estatura", SqlDbType.Decimal) { Value = (object?)p.Estatura ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Alergias", SqlDbType.VarChar) { Value = (object?)p.Alergias ?? DBNull.Value });
        }

        private static PacienteDto Map(SqlDataReader r)
        {
            return new PacienteDto
            {
                IdPaciente = r.GetInt32(r.GetOrdinal("IdPaciente")),
                Identificacion = r.GetString(r.GetOrdinal("Identificacion")),
                Nombre = r.GetString(r.GetOrdinal("Nombre")),
                Apellidos = r.GetString(r.GetOrdinal("Apellidos")),
                FechaNacimiento = r.GetDateTime(r.GetOrdinal("FechaNacimiento")),
                AntecedentesMedicos = r["AntecedentesMedicos"] as string,
                EsRecienNacido = r.GetBoolean(r.GetOrdinal("EsRecienNacido")),
                IdSexoBiologico = r["IdSexoBiologico"] == DBNull.Value ? null : r.GetInt32(r.GetOrdinal("IdSexoBiologico")),
                SexoBiologico = r["SexoBiologico"] as string,
                IdIdentidadGenero = r["IdIdentidadGenero"] == DBNull.Value ? null : r.GetInt32(r.GetOrdinal("IdIdentidadGenero")),
                IdentidadGenero = r["IdentidadGenero"] as string,
                IdTipoSangre = r["IdTipoSangre"] == DBNull.Value ? null : r.GetInt32(r.GetOrdinal("IdTipoSangre")),
                TipoSangre = r["Tipo_Sangre"] as string,
                Peso = r["Peso"] == DBNull.Value ? null : r.GetDecimal(r.GetOrdinal("Peso")),
                Estatura = r["Estatura"] == DBNull.Value ? null : r.GetDecimal(r.GetOrdinal("Estatura")),
                Alergias = r["Alergias"] as string
            };
        }

        // Guarda el expediente
        public RespuestaCrud GuardarExpediente(ExpedienteDto e)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Paciente_GuardarExpediente";

            cmd.Parameters.Add(new SqlParameter("@IdPaciente", SqlDbType.Int) { Value = e.IdPaciente });
            cmd.Parameters.Add(new SqlParameter("@AntecedentesMedicos", SqlDbType.NVarChar) { Value = (object?)e.AntecedentesMedicos ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IdTipoSangre", SqlDbType.Int) { Value = (object?)e.IdTipoSangre ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Peso", SqlDbType.Decimal) { Value = (object?)e.Peso ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Estatura", SqlDbType.Decimal) { Value = (object?)e.Estatura ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Alergias", SqlDbType.VarChar) { Value = (object?)e.Alergias ?? DBNull.Value });

            conn.Open();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Expediente guardado correctamente." };
        }

        //Tipo de sangre
        public List<TipoSangreDto> ListarTiposSangre()
        {
            var lista = new List<TipoSangreDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT IdTipoSangre, Descripcion FROM dbo.Tipos_Sangre ORDER BY Descripcion;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(new TipoSangreDto
                {
                    IdTipoSangre = reader.GetInt32(0),
                    Descripcion = reader.GetString(1)
                });
            return lista;
        }

        public List<SexoBiologicoDto> ListarSexosBiologicos()
        {
            var lista = new List<SexoBiologicoDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT IdSexoBiologico, Descripcion FROM dbo.Sexos_Biologicos ORDER BY Descripcion;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(new SexoBiologicoDto { IdSexoBiologico = reader.GetInt32(0), Descripcion = reader.GetString(1) });
            return lista;
        }

        public List<IdentidadGeneroDto> ListarIdentidadesGenero()
        {
            var lista = new List<IdentidadGeneroDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT IdIdentidadGenero, Descripcion FROM dbo.Identidades_Genero ORDER BY Descripcion;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(new IdentidadGeneroDto { IdIdentidadGenero = reader.GetInt32(0), Descripcion = reader.GetString(1) });
            return lista;
        }
    }
}
