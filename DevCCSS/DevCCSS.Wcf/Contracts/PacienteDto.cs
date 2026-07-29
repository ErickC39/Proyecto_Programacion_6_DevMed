using System.Runtime.Serialization;

namespace DevCCSS.Wcf.Contracts
{
    
    [DataContract(Namespace = "http://devccss/contracts")]
    public class PacienteDto
    {
        [DataMember] public int IdPaciente { get; set; }
        [DataMember] public string Identificacion { get; set; } = string.Empty;
        [DataMember] public string Nombre { get; set; } = string.Empty;
        [DataMember] public string Apellidos { get; set; } = string.Empty;
        [DataMember] public DateTime FechaNacimiento { get; set; }
        [DataMember] public string? AntecedentesMedicos { get; set; }
        [DataMember] public bool EsRecienNacido { get; set; }
        [DataMember] public int? IdSexoBiologico { get; set; }
        [DataMember] public string? SexoBiologico { get; set; }
        [DataMember] public int? IdIdentidadGenero { get; set; }
        [DataMember] public string? IdentidadGenero { get; set; }
        [DataMember] public int? IdTipoSangre { get; set; }
        [DataMember] public string? TipoSangre { get; set; }
        [DataMember] public decimal? Peso { get; set; }
        [DataMember] public decimal? Estatura { get; set; }
        [DataMember] public string? Alergias { get; set; }
    }
}
