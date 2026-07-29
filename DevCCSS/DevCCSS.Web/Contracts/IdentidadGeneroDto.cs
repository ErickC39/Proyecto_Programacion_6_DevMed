using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class IdentidadGeneroDto
    {
        [DataMember] public int IdIdentidadGenero { get; set; }
        [DataMember] public string Descripcion { get; set; } = string.Empty;
    }
}