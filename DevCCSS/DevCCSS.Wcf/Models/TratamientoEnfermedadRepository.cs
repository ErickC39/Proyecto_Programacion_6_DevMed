using DevCCSS.Wcf.Contracts;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    public class TratamientoEnfermedadRepository
    {
        private readonly string _connectionString;

        public TratamientoEnfermedadRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        public List<TratamientoEnfermedadDto> ListarPorEnfermedad(int idEnfermedad)
        {
            var lista = new List<TratamientoEnfermedadDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_TratamientoEnfermedad WHERE IdEnfermedad = @IdEnfermedad ORDER BY NombreMedicamento;";
            cmd.Parameters.Add(new SqlParameter("@IdEnfermedad", SqlDbType.Int) { Value = idEnfermedad });
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new TratamientoEnfermedadDto
                {
                    IdEnfermedad = reader.GetInt32(reader.GetOrdinal("IdEnfermedad")),
                    NombreEnfermedad = reader.GetString(reader.GetOrdinal("NombreEnfermedad")),
                    IdMedicamento = reader.GetInt32(reader.GetOrdinal("IdMedicamento")),
                    NombreMedicamento = reader.GetString(reader.GetOrdinal("NombreMedicamento")),
                    ObservacionEspecifica = reader["ObservacionEspecifica"] as string
                });
            }
            return lista;
        }

        public RespuestaCrud Asignar(int idEnfermedad, int idMedicamento, string? observacion)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_TratamientoEnfermedad_Asignar";
            cmd.Parameters.Add(new SqlParameter("@IdEnfermedad", SqlDbType.Int) { Value = idEnfermedad });
            cmd.Parameters.Add(new SqlParameter("@IdMedicamento", SqlDbType.Int) { Value = idMedicamento });
            cmd.Parameters.Add(new SqlParameter("@ObservacionEspecifica", SqlDbType.NVarChar, 255) { Value = (object?)observacion ?? DBNull.Value });
            var pMsg = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
            var pCode = new SqlParameter("@CodigoSalida", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pMsg);
            cmd.Parameters.Add(pCode);
            conn.Open();
            cmd.ExecuteNonQuery();
            int codigo = pCode.Value == DBNull.Value ? -1 : (int)pCode.Value;
            return new RespuestaCrud { Ok = codigo == 0, Mensaje = pMsg.Value?.ToString() ?? string.Empty };
        }

        public RespuestaCrud Quitar(int idEnfermedad, int idMedicamento)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_TratamientoEnfermedad_Quitar";
            cmd.Parameters.Add(new SqlParameter("@IdEnfermedad", SqlDbType.Int) { Value = idEnfermedad });
            cmd.Parameters.Add(new SqlParameter("@IdMedicamento", SqlDbType.Int) { Value = idMedicamento });
            var pMsg = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
            var pCode = new SqlParameter("@CodigoSalida", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pMsg);
            cmd.Parameters.Add(pCode);
            conn.Open();
            cmd.ExecuteNonQuery();
            int codigo = pCode.Value == DBNull.Value ? -1 : (int)pCode.Value;
            return new RespuestaCrud { Ok = codigo == 0, Mensaje = pMsg.Value?.ToString() ?? string.Empty };
        }
    }
}
