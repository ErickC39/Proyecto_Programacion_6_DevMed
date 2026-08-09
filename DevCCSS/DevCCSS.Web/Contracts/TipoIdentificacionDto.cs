using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class TipoIdentificacionDto
    {
        [DataMember] public int IdTipoIdentificacion { get; set; }
        [DataMember] public string Descripcion { get; set; } = string.Empty;
    }
}
