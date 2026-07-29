using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class EmpleadoDto
    {
        [DataMember] public int IdEmpleado { get; set; }
        [DataMember] public string Identificacion { get; set; } = string.Empty;
        [DataMember] public string Nombre { get; set; } = string.Empty;
        [DataMember] public string Apellidos { get; set; } = string.Empty;
        [DataMember] public string? Especialidad { get; set; }
        [DataMember] public decimal SalarioPorHora { get; set; }
        [DataMember] public int IdUsuario { get; set; }
        [DataMember] public string? UsuarioAsignado { get; set; }
        [DataMember] public int? IdPacienteVinculado { get; set; }
    }
}
