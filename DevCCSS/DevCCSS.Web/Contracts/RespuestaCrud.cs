using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class RespuestaCrud
    {
        [DataMember] public bool Ok { get; set; }
        [DataMember] public string Mensaje { get; set; } = string.Empty;
        [DataMember] public int IdGenerado { get; set; }
    }
}
