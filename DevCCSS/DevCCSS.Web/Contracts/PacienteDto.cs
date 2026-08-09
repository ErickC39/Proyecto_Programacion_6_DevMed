using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class PacienteDto : IValidatableObject
    {
        // Coinciden con el orden de siembra del catalogo Tipos_Identificacion.
        public const int TIPO_NACIONAL = 1;
        public const int TIPO_DIMEX = 2;
        public const int TIPO_PASAPORTE = 3;

        [DataMember] public int IdPaciente { get; set; }
        [DataMember] public string Identificacion { get; set; } = string.Empty;
        [DataMember] public int? IdTipoIdentificacion { get; set; }
        [DataMember] public string? TipoIdentificacion { get; set; }
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

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var valor = Identificacion ?? string.Empty;
            switch (IdTipoIdentificacion)
            {
                case TIPO_NACIONAL:
                    if (!Regex.IsMatch(valor, @"^\d-\d{4}-\d{4}$"))
                        yield return new ValidationResult(
                            "La cedula nacional debe tener el formato 1-1111-1111.",
                            new[] { nameof(Identificacion) });
                    break;
                case TIPO_DIMEX:
                    if (!Regex.IsMatch(valor, @"^\d{1,12}$"))
                        yield return new ValidationResult(
                            "El DIMEX debe ser numerico, de hasta 12 digitos.",
                            new[] { nameof(Identificacion) });
                    break;
                case TIPO_PASAPORTE:
                    if (!Regex.IsMatch(valor, @"^[A-Za-z0-9]{9}$"))
                        yield return new ValidationResult(
                            "El pasaporte debe tener 9 caracteres alfanumericos.",
                            new[] { nameof(Identificacion) });
                    break;
            }
        }
    }
}
