using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class VisitanteDto
    {
        [DataMember] public int IdVisita { get; set; }
        [DataMember] public string Identificacion { get; set; } = string.Empty;
        [DataMember] public string NombreCompleto { get; set; } = string.Empty;
        [DataMember] public int IdPacienteAVisitar { get; set; }
        [DataMember] public string? PacienteVisitado { get; set; }
        [DataMember] public DateTime FechaHoraEntrada { get; set; }
        [DataMember] public DateTime? FechaHoraSalida { get; set; }
        [DataMember] public int? MinutosEstancia { get; set; }
    }
}
