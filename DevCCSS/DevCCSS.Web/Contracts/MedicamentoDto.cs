using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class MedicamentoDto
    {
        [DataMember] public int IdMedicamento { get; set; }
        [DataMember] public string Nombre { get; set; } = string.Empty;
        [DataMember] public string IndicacionesUso { get; set; } = string.Empty;
        [DataMember] public string? Restricciones { get; set; }
        [DataMember] public string? HorasAplicacionRecomendada { get; set; }
        [DataMember] public int? IdProducto { get; set; }
        [DataMember] public int? CantidadStock { get; set; }
        [DataMember] public decimal? PrecioUnitario { get; set; }
    }
}