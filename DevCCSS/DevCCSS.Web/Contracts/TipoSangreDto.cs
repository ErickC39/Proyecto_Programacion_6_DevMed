using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class TipoSangreDto
    {
        [DataMember] public int IdTipoSangre { get; set; }
        [DataMember] public string Descripcion { get; set; } = string.Empty;
    }
}