using System.Runtime.Serialization;

namespace DevCCSS.Wcf.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class EnfermedadDto
    {
        [DataMember] public int IdEnfermedad { get; set; }
        [DataMember] public string Nombre { get; set; } = string.Empty;
        [DataMember] public string? Descripcion { get; set; }
        [DataMember] public string? RecomendacionesGenerales { get; set; }
    }
}
