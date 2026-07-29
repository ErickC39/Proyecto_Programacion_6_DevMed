using System.Runtime.Serialization;

namespace DevCCSS.Wcf.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class SexoBiologicoDto
    {
        [DataMember] public int IdSexoBiologico { get; set; }
        [DataMember] public string Descripcion { get; set; } = string.Empty;
    }
}