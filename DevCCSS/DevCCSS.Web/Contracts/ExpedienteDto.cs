using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class ExpedienteDto
    {
        [DataMember] public int IdPaciente { get; set; }
        [DataMember] public string? AntecedentesMedicos { get; set; }
        [DataMember] public int? IdTipoSangre { get; set; }
        [DataMember] public decimal? Peso { get; set; }
        [DataMember] public decimal? Estatura { get; set; }
        [DataMember] public string? Alergias { get; set; }

    }
}