using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class LoginResponse
    {
        [DataMember] public bool Success { get; set; }
        [DataMember] public int IdUsuario { get; set; }
        [DataMember] public string Username { get; set; } = string.Empty;
        [DataMember] public string Nombre { get; set; } = string.Empty;
        [DataMember] public bool Activo { get; set; }
        [DataMember] public string[] Roles { get; set; } = Array.Empty<string>();
        [DataMember] public string Message { get; set; } = string.Empty;
    }
}
