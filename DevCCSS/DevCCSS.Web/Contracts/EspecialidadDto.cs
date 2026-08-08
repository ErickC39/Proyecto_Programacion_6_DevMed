using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class EspecialidadDto
    {
        [DataMember]
        public int IdEspecialidad { get; set; }

        [DataMember]
        public string Nombre { get; set; } = string.Empty;
    }
}
