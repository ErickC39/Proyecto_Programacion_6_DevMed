using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class TratamientoEnfermedadDto
    {
        [DataMember] public int IdEnfermedad { get; set; }
        [DataMember] public string NombreEnfermedad { get; set; } = string.Empty;
        [DataMember] public int IdMedicamento { get; set; }
        [DataMember] public string NombreMedicamento { get; set; } = string.Empty;
        [DataMember] public string? ObservacionEspecifica { get; set; }
    }
}
