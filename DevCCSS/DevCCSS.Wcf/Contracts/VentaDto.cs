using System.Runtime.Serialization;

namespace DevCCSS.Wcf.Contracts
{
    /*[DataContract(Namespace = "http://devccss/contracts")]
    public class ProductoDto
    {
        [DataMember] public int IdProducto { get; set; }
        [DataMember] public string NombreProducto { get; set; } = string.Empty;
        [DataMember] public string? Descripcion { get; set; }
        [DataMember] public decimal PrecioUnitario { get; set; }
        [DataMember] public int CantidadStock { get; set; }
    }*/

    [DataContract(Namespace = "http://devccss/contracts")]
    public class DetalleFacturaDto
    {
        [DataMember] public int IdProducto { get; set; }
        [DataMember] public string? NombreProducto { get; set; }
        [DataMember] public int Cantidad { get; set; }
        [DataMember] public decimal PrecioUnitario { get; set; }
        [DataMember] public decimal SubtotalLinea { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class VentaDto
    {
        [DataMember] public int IdFactura { get; set; }
        [DataMember] public DateTime FechaFactura { get; set; }
        [DataMember] public string? ClienteNombreCompleto { get; set; }
        [DataMember] public string? ClienteIdentificacion { get; set; }
        [DataMember] public string? EmpleadoNombreCompleto { get; set; }
        [DataMember] public decimal Subtotal { get; set; }
        [DataMember] public decimal Descuento { get; set; }
        [DataMember] public decimal Impuestos { get; set; }
        [DataMember] public decimal Total { get; set; }
        [DataMember] public string MetodoPago { get; set; } = "Efectivo";
        [DataMember] public List<DetalleFacturaDto> Detalle { get; set; } = new();
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class ItemVentaDto
    {
        [DataMember] public int IdProducto { get; set; }
        [DataMember] public int Cantidad { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class CrearVentaDto
    {
        [DataMember] public string? PacienteIdentificacion { get; set; }
        [DataMember] public int? IdEmpleadoAtiende { get; set; }
        [DataMember] public string MetodoPago { get; set; } = "Efectivo";
        [DataMember] public decimal Descuento { get; set; } = 0;
        [DataMember] public List<ItemVentaDto> Items { get; set; } = new();
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class RespuestaVenta
    {
        [DataMember] public bool Ok { get; set; }
        [DataMember] public string Mensaje { get; set; } = string.Empty;
        [DataMember] public int IdFacturaGenerada { get; set; }
        [DataMember] public decimal Total { get; set; }
        [DataMember] public int CodigoSalida { get; set; }
    }
}