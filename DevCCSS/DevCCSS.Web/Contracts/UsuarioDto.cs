using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class UsuarioDto : IValidatableObject
    {
        public const int ROL_ADMINISTRADOR = 1;

        [DataMember] public int IdUsuario { get; set; }
        [DataMember] public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Los apellidos son obligatorios.")]
        [DataMember] public string? Apellidos { get; set; }

        [DataMember] public string Username { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 8, ErrorMessage = "La contrasenia debe tener al menos 8 caracteres.")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d).+$", ErrorMessage = "La contrasenia debe incluir al menos una letra y un numero.")]
        [DataMember] public string? Password { get; set; }   // solo se usa al crear
        [DataMember] public int IdRol { get; set; }
        [DataMember] public string? Rol { get; set; }        // solo para mostrar
        [DataMember] public bool Activo { get; set; }
        [DataMember] public DateTime? FechaCreacion { get; set; }

        // No viaja por WCF (sin [DataMember]): solo se usa al crear un
        // usuario para generar tambien su registro de Empleado estandar
        // (ver UsuariosController.Create), cuando el rol no es Administrador.
        public decimal? SalarioPorHora { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (IdUsuario == 0 && IdRol != 0 && IdRol != ROL_ADMINISTRADOR)
            {
                if (SalarioPorHora is null || SalarioPorHora <= 0)
                {
                    yield return new ValidationResult(
                        "El salario por hora debe ser mayor a cero para crear el empleado asociado.",
                        new[] { nameof(SalarioPorHora) });
                }
            }
        }
    }
}
