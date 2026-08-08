using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Infrastructure;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    public class VentaRepository
    {
        private readonly string _connectionString;

        public VentaRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        public List<ProductoDto> ListarProductos()
        {
            var lista = new List<ProductoDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            // Solo se venden productos que NO esten marcados como insumo medico
            // (los insumos son de uso clinico interno, no articulos de venta/factura).
            cmd.CommandText = "SELECT IdProducto, NombreProducto, Descripcion, PrecioUnitario, CantidadStock, EsInsumoMedico, IdMedicamento FROM dbo.vw_Inventario WHERE ISNULL(EsInsumoMedico, 0) = 0 ORDER BY NombreProducto;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new ProductoDto
                {
                    IdProducto = reader.GetInt32(reader.GetOrdinal("IdProducto")),
                    NombreProducto = reader.GetString(reader.GetOrdinal("NombreProducto")),
                    Descripcion = reader["Descripcion"] as string,
                    PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                    CantidadStock = reader["CantidadStock"] == DBNull.Value ? 0 : reader.GetInt32(reader.GetOrdinal("CantidadStock")),
                    EsInsumoMedico = reader["EsInsumoMedico"] != DBNull.Value && reader.GetBoolean(reader.GetOrdinal("EsInsumoMedico")),
                    IdMedicamento = reader["IdMedicamento"] == DBNull.Value ? null : reader.GetInt32(reader.GetOrdinal("IdMedicamento"))
                });
            }
            return lista;
        }

        public List<VentaDto> Listar()
        {
            var lista = new List<VentaDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_Ventas ORDER BY FechaFactura DESC;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(MapVenta(reader));
            return lista;
        }

        public VentaDto? ObtenerPorId(int id)
        {
            VentaDto? venta;
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM dbo.vw_Ventas WHERE IdFactura = @Id;";
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                conn.Open();
                using var reader = cmd.ExecuteReader();
                if (!reader.Read()) return null;
                venta = MapVenta(reader);
            }

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM dbo.vw_DetallesFactura WHERE IdFactura = @Id;";
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = id });
                conn.Open();
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    venta.Detalle.Add(new DetalleFacturaDto
                    {
                        IdProducto = reader.GetInt32(reader.GetOrdinal("IdProducto")),
                        NombreProducto = reader.GetString(reader.GetOrdinal("NombreProducto")),
                        Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                        PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                        SubtotalLinea = reader.GetDecimal(reader.GetOrdinal("SubtotalLinea"))
                    });
                }
            }

            return venta;
        }

        public RespuestaVenta Registrar(CrearVentaDto dto)
        {
            if (dto == null)
                return new RespuestaVenta { Ok = false, Mensaje = "Solicitud invalida.", CodigoSalida = -1 };

            if (dto.Items == null)
                dto.Items = new List<ItemVentaDto>();

            int? idPaciente = null;
            if (!string.IsNullOrWhiteSpace(dto.PacienteIdentificacion))
            {
                using var connBuscar = new SqlConnection(_connectionString);
                using var cmdBuscar = connBuscar.CreateCommand();
                cmdBuscar.CommandText = "SELECT IdPaciente FROM dbo.Pacientes WHERE Identificacion = @Id;";
                cmdBuscar.Parameters.Add(new SqlParameter("@Id", SqlDbType.NVarChar, 50) { Value = dto.PacienteIdentificacion });
                connBuscar.Open();
                var result = cmdBuscar.ExecuteScalar();
                if (result != null) idPaciente = (int)result;
                else return new RespuestaVenta { Ok = false, Mensaje = "No se encontro ningun paciente con esa identificacion.", CodigoSalida = 2 };
            }

            var tabla = new DataTable();
            tabla.Columns.Add("IdProducto", typeof(int));
            tabla.Columns.Add("Cantidad", typeof(int));
            foreach (var item in dto.Items)
                tabla.Rows.Add(item.IdProducto, item.Cantidad);

            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Venta_Registrar";

            cmd.Parameters.Add(new SqlParameter("@IdPaciente", SqlDbType.Int) { Value = (object?)idPaciente ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IdEmpleadoAtiende", SqlDbType.Int) { Value = (object?)dto.IdEmpleadoAtiende ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@MetodoPago", SqlDbType.NVarChar, 20) { Value = dto.MetodoPago });

            // OJO: precision/scale explicitos -- sin esto ADO.NET usa Scale=0 por defecto
            // y SQL Server truena al convertir un DECIMAL(10,2) real hacia ese parametro.
            cmd.Parameters.Add(new SqlParameter("@Descuento", SqlDbType.Decimal) { Precision = 10, Scale = 2, Value = dto.Descuento });

            var pDetalle = new SqlParameter("@Detalle", SqlDbType.Structured) { TypeName = "dbo.DetalleFacturaType", Value = tabla };
            cmd.Parameters.Add(pDetalle);

            var pIdFactura = new SqlParameter("@IdFacturaGenerada", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var pTotal = new SqlParameter("@Total", SqlDbType.Decimal) { Precision = 10, Scale = 2, Direction = ParameterDirection.Output };
            var pMsg = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
            var pCodigo = new SqlParameter("@CodigoSalida", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pIdFactura);
            cmd.Parameters.Add(pTotal);
            cmd.Parameters.Add(pMsg);
            cmd.Parameters.Add(pCodigo);

            conn.Open();
            conn.EstablecerUsuarioAuditoria();
            cmd.ExecuteNonQuery();

            int codigo = pCodigo.Value == DBNull.Value ? -1 : (int)pCodigo.Value;

            return new RespuestaVenta
            {
                Ok = codigo == 0,
                Mensaje = pMsg.Value?.ToString() ?? string.Empty,
                IdFacturaGenerada = pIdFactura.Value == DBNull.Value ? 0 : (int)pIdFactura.Value,
                Total = pTotal.Value == DBNull.Value ? 0 : (decimal)pTotal.Value,
                CodigoSalida = codigo
            };
        }

        private static VentaDto MapVenta(SqlDataReader r) => new VentaDto
        {
            IdFactura = r.GetInt32(r.GetOrdinal("IdFactura")),
            FechaFactura = r.GetDateTime(r.GetOrdinal("FechaFactura")),
            ClienteNombreCompleto = r["ClienteNombreCompleto"] as string,
            ClienteIdentificacion = r["ClienteIdentificacion"] as string,
            EmpleadoNombreCompleto = r["EmpleadoNombreCompleto"] as string,
            Subtotal = r.GetDecimal(r.GetOrdinal("Subtotal")),
            Descuento = r.GetDecimal(r.GetOrdinal("Descuento")),
            Impuestos = r.GetDecimal(r.GetOrdinal("Impuestos")),
            Total = r.GetDecimal(r.GetOrdinal("Total")),
            MetodoPago = r.GetString(r.GetOrdinal("MetodoPago"))
        };
    }
}