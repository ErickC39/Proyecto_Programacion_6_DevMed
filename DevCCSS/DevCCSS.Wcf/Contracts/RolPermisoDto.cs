using System.Runtime.Serialization;

namespace DevCCSS.Wcf.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class RolPermisoDto
    {
        [DataMember] public int IdRolPermiso { get; set; }
        [DataMember] public int IdRol { get; set; }
        [DataMember] public string NombreRol { get; set; } = string.Empty;
        [DataMember] public string Modulo { get; set; } = string.Empty;
        [DataMember] public bool PuedeVer { get; set; }
        [DataMember] public bool PuedeCrear { get; set; }
        [DataMember] public bool PuedeEditar { get; set; }
        [DataMember] public bool PuedeEliminar { get; set; }
    }
}
