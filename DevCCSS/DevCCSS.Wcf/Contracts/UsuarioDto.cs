using System.Runtime.Serialization;

namespace DevCCSS.Wcf.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class UsuarioDto
    {
        [DataMember] public int IdUsuario { get; set; }
        [DataMember] public string Username { get; set; } = string.Empty;
        [DataMember] public string? Password { get; set; }   // solo se usa al crear
        [DataMember] public int IdRol { get; set; }
        [DataMember] public string? Rol { get; set; }        // solo para mostrar
        [DataMember] public bool Activo { get; set; }
        [DataMember] public DateTime? FechaCreacion { get; set; }
    }
}
