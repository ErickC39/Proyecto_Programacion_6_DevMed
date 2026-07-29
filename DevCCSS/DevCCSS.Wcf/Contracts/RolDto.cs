using System.Runtime.Serialization;

namespace DevCCSS.Wcf.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class RolDto
    {
        [DataMember] public int IdRol { get; set; }
        [DataMember] public string NombreRol { get; set; } = string.Empty;
    }
}
