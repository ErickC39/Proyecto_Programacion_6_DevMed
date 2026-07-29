using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class NacimientoDto
    {
        [DataMember] public int IdPaciente { get; set; }
        [DataMember] public string Identificacion { get; set; } = string.Empty;
        [DataMember] public string Nombre { get; set; } = string.Empty;
        [DataMember] public string Apellidos { get; set; } = string.Empty;
        [DataMember] public DateTime FechaNacimiento { get; set; }
        [DataMember] public int? IdSexoBiologico { get; set; }
        [DataMember] public string? SexoBiologico { get; set; }
        [DataMember] public int? IdTipoSangre { get; set; }
        [DataMember] public string? TipoSangre { get; set; }
        [DataMember] public decimal? Peso { get; set; }
        [DataMember] public decimal? Estatura { get; set; }
        [DataMember] public string? Alergias { get; set; }
        [DataMember] public string? AntecedentesMedicos { get; set; }
        [DataMember] public int? IdMadre { get; set; }
        [DataMember] public string? NombreMadre { get; set; }
        [DataMember] public int? Apgar1Min { get; set; }
        [DataMember] public int? Apgar5Min { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class ApgarDto
    {
        [DataMember] public int Minuto { get; set; }
        [DataMember] public int Apariencia { get; set; }
        [DataMember] public int Pulso { get; set; }
        [DataMember] public int GestoRespuesta { get; set; }
        [DataMember] public int Actividad { get; set; }
        [DataMember] public int Respiracion { get; set; }
        [DataMember] public int Total { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class TamizajeDto
    {
        [DataMember] public string TipoTamizaje { get; set; } = string.Empty;
        [DataMember] public bool Realizado { get; set; }
        [DataMember] public string? Resultado { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class RegistrarNacimientoDto
    {
        [DataMember] public int IdPaciente { get; set; }
        [DataMember] public string Nombre { get; set; } = string.Empty;
        [DataMember] public string Apellidos { get; set; } = string.Empty;
        [DataMember] public DateTime FechaNacimiento { get; set; }
        [DataMember] public int? IdSexoBiologico { get; set; }
        [DataMember] public int? IdTipoSangre { get; set; }
        [DataMember] public decimal? Peso { get; set; }
        [DataMember] public decimal? Estatura { get; set; }
        [DataMember] public string? Alergias { get; set; }
        [DataMember] public string? AntecedentesMedicos { get; set; }
        [DataMember] public int? IdMadre { get; set; }
        [DataMember] public decimal? PerimetroCefalico { get; set; }
        [DataMember] public decimal? PerimetroToracico { get; set; }
        [DataMember] public decimal? Temperatura { get; set; }
        [DataMember] public int? FrecuenciaCardiaca { get; set; }
        [DataMember] public int? FrecuenciaRespiratoria { get; set; }
        [DataMember] public string? Reflejos { get; set; }
        [DataMember] public string? ColoracionPiel { get; set; }
        [DataMember] public string? ObservacionesGenerales { get; set; }
        [DataMember] public List<ApgarDto> Apgar { get; set; } = new();
        [DataMember] public List<TamizajeDto> Tamizajes { get; set; } = new();
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class NacimientoDetalleDto
    {
        [DataMember] public NacimientoDto? Datos { get; set; }
        [DataMember] public List<ApgarDto> Apgar { get; set; } = new();
        [DataMember] public List<TamizajeDto> Tamizajes { get; set; } = new();
        [DataMember] public decimal? PerimetroCefalico { get; set; }
        [DataMember] public decimal? PerimetroToracico { get; set; }
        [DataMember] public decimal? Temperatura { get; set; }
        [DataMember] public int? FrecuenciaCardiaca { get; set; }
        [DataMember] public int? FrecuenciaRespiratoria { get; set; }
        [DataMember] public string? Reflejos { get; set; }
        [DataMember] public string? ColoracionPiel { get; set; }
        [DataMember] public string? ObservacionesGenerales { get; set; }
    }
}