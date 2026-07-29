using DevCCSS.Wcf.Contracts;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    
    public class MedicamentoRepository
    {
        private readonly string _connectionString;

        public MedicamentoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        // LISTAR
        public List<MedicamentoDto> Listar()
        {
            var lista = new List<MedicamentoDto>();

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Medicamentos ORDER BY Nombre;";

            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Map(reader));

            return lista;
        }

        // OBTENER POR ID
        public MedicamentoDto? ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Medicamentos WHERE IdMedicamento = @Id;";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });

            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return Map(reader);
        }

        // CREAR 
        public RespuestaCrud Crear(MedicamentoDto m)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Medicamento_Crear";
            AgregarParametros(cmd, m, incluirId: false);

            var pId = new SqlParameter("@IdGenerado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pId);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud
            {
                Ok = true,
                Mensaje = "Medicamento creado correctamente.",
                IdGenerado = pId.Value == DBNull.Value ? 0 : (int)pId.Value
            };
        }

        // ACTUALIZAR
        public RespuestaCrud Actualizar(MedicamentoDto m)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Medicamento_Actualizar";
            AgregarParametros(cmd, m, incluirId: true);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud { Ok = true, Mensaje = "Medicamento actualizado correctamente." };
        }

        // ELIMINAR
        public RespuestaCrud Eliminar(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Medicamento_Eliminar";
            cmd.Parameters.Add(new SqlParameter("@IdMedicamento", SqlDbType.Int) { Value = id });

            conn.Open();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud { Ok = true, Mensaje = "Medicamento eliminado correctamente." };
        }

        //  helpers
        private static void AgregarParametros(SqlCommand cmd, MedicamentoDto m, bool incluirId)
        {
            if (incluirId)
                cmd.Parameters.Add(new SqlParameter("@IdMedicamento", SqlDbType.Int) { Value = m.IdMedicamento });

            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 150) { Value = m.Nombre });
            cmd.Parameters.Add(new SqlParameter("@IndicacionesUso", SqlDbType.NVarChar) { Value = m.IndicacionesUso });
            cmd.Parameters.Add(new SqlParameter("@Restricciones", SqlDbType.NVarChar) { Value = (object?)m.Restricciones ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@HorasAplicacionRecomendada", SqlDbType.NVarChar, 100) { Value = (object?)m.HorasAplicacionRecomendada ?? DBNull.Value });
        }

        private static MedicamentoDto Map(SqlDataReader r)
        {
            return new MedicamentoDto
            {
                IdMedicamento = r.GetInt32(r.GetOrdinal("IdMedicamento")),
                Nombre = r.GetString(r.GetOrdinal("Nombre")),
                IndicacionesUso = r.GetString(r.GetOrdinal("IndicacionesUso")),
                Restricciones = r["Restricciones"] as string,
                HorasAplicacionRecomendada = r["HorasAplicacionRecomendada"] as string
            };
        }
    }
}