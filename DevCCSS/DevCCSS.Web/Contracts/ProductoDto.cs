using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class ProductoDto
    {
        [DataMember] public int IdProducto { get; set; }
        [DataMember] public string NombreProducto { get; set; } = string.Empty;
        [DataMember] public string? Descripcion { get; set; }
        [DataMember] public int CantidadStock { get; set; }
        [DataMember] public decimal PrecioUnitario { get; set; }
        [DataMember] public bool EsInsumoMedico { get; set; }
    }
}
