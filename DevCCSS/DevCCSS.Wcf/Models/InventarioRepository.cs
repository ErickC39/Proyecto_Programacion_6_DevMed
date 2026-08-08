using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    public class InventarioRepository
    {
        private readonly string _connectionString;

        public InventarioRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        public List<ProductoDto> Listar()
        {
            var lista = new List<ProductoDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Inventario ORDER BY NombreProducto;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Map(reader));
            return lista;
        }

        public ProductoDto? ObtenerPorId(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Inventario WHERE IdProducto = @Id;";
            cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
            conn.Open();
            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;
            return Map(reader);
        }

        public RespuestaCrud Crear(ProductoDto m)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Inventario_Crear";
            AgregarParametros(cmd, m, incluirId: false);
            var pId = new SqlParameter("@IdGenerado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pId);
            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Registro creado correctamente.", IdGenerado = pId.Value == DBNull.Value ? 0 : (int)pId.Value };
        }

        public RespuestaCrud Actualizar(ProductoDto m)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Inventario_Actualizar";
            AgregarParametros(cmd, m, incluirId: true);
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
            cmd.CommandText = "dbo.sp_Inventario_Eliminar";
            cmd.Parameters.Add(new SqlParameter("@IdProducto", SqlDbType.Int) { Value = id });
            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();
            return new RespuestaCrud { Ok = true, Mensaje = "Registro eliminado correctamente." };
        }

        private static void AgregarParametros(SqlCommand cmd, ProductoDto m, bool incluirId)
        {
            if (incluirId)
                cmd.Parameters.Add(new SqlParameter("@IdProducto", SqlDbType.Int) { Value = m.IdProducto });
            cmd.Parameters.Add(new SqlParameter("@NombreProducto", SqlDbType.NVarChar, 150) { Value = m.NombreProducto });
            cmd.Parameters.Add(new SqlParameter("@Descripcion", SqlDbType.NVarChar, 255) { Value = (object?)m.Descripcion ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@CantidadStock", SqlDbType.Int) { Value = m.CantidadStock });
            cmd.Parameters.Add(new SqlParameter("@PrecioUnitario", SqlDbType.Decimal) { Value = m.PrecioUnitario });
            cmd.Parameters.Add(new SqlParameter("@EsInsumoMedico", SqlDbType.Bit) { Value = m.EsInsumoMedico });
            cmd.Parameters.Add(new SqlParameter("@StockMinimo", SqlDbType.Int) { Value = m.StockMinimo });
            cmd.Parameters.Add(new SqlParameter("@IdMedicamento", SqlDbType.Int) { Value = (object?)m.IdMedicamento ?? DBNull.Value });
        }

        private static ProductoDto Map(SqlDataReader r)
        {
            return new ProductoDto
            {
                IdProducto = r.GetInt32(r.GetOrdinal("IdProducto")),
                NombreProducto = r.GetString(r.GetOrdinal("NombreProducto")),
                Descripcion = r["Descripcion"] as string,
                CantidadStock = r.GetInt32(r.GetOrdinal("CantidadStock")),
                PrecioUnitario = r.GetDecimal(r.GetOrdinal("PrecioUnitario")),
                EsInsumoMedico = r.GetBoolean(r.GetOrdinal("EsInsumoMedico")),
                StockMinimo = r.GetInt32(r.GetOrdinal("StockMinimo")),
                IdMedicamento = r["IdMedicamento"] == DBNull.Value ? null : r.GetInt32(r.GetOrdinal("IdMedicamento")),
                NombreMedicamentoVinculado = r["NombreMedicamentoVinculado"] as string
            };
        }
    }
}
