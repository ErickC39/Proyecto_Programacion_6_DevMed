using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class CitaDto
    {
        [DataMember] public int IdCita { get; set; }
        [DataMember] public int IdPaciente { get; set; }
        [DataMember] public string PacienteIdentificacion { get; set; } = string.Empty;
        [DataMember] public string PacienteNombreCompleto { get; set; } = string.Empty;
        [DataMember] public int IdEmpleado_Medico { get; set; }
        [DataMember] public string MedicoNombreCompleto { get; set; } = string.Empty;
        [DataMember] public string? Especialidad { get; set; }
        [DataMember] public DateTime FechaHoraCita { get; set; }
        [DataMember] public DateTime? FechaHoraLlegada { get; set; }
        [DataMember] public int TiempoEsperaMinutos { get; set; }
        [DataMember] public string EstadoCita { get; set; } = "Agendada";
        [DataMember] public string? ResultadoConsulta { get; set; }
        [DataMember] public bool RequiereControl { get; set; }
        [DataMember] public string? PrioridadCita { get; set; }
        [DataMember] public bool EsCitaControl { get; set; }
        [DataMember] public bool CitaPreviaEsControl { get; set; }
        [DataMember] public bool FueReagendadaPorEmergencia { get; set; }
        [DataMember] public string? MensajeReagendo { get; set; }
        [DataMember] public int? IdTipoHabitacionRequerido { get; set; }
        [DataMember] public string? TipoHabitacionRequerido { get; set; }
        [DataMember] public int? IdHabitacionAsignada { get; set; }
        [DataMember] public string? NumeroHabitacionAsignada { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class AgendarCitaDto
    {

        [Required(ErrorMessage = "Debe ingresar la identificacion del paciente.")]
        [DataMember] public string PacienteIdentificacion { get; set; } = string.Empty;


        [Required(ErrorMessage = "Debe seleccionar un medico.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un medico valido.")]
        [DataMember] public int IdMedico { get; set; }

        [DataMember]
        [DataType(DataType.DateTime)]
        public DateTime FechaHoraCita { get; set; }

        [DataMember] public string PrioridadCita { get; set; } = "Normal";
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class AgendarEmergenciaDto
    {
        [Required(ErrorMessage = "Debe seleccionar el empleado vinculado al paciente de emergencia.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un empleado valido.")]
        [DataMember] public int IdEmpleado { get; set; }
        [Required(ErrorMessage = "Debe seleccionar un medico.")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un medico valido.")]
        [DataMember] public int IdMedico { get; set; }
        [DataMember] public DateTime FechaHoraCita { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class FinalizarCitaDto : IValidatableObject
    {
        [DataMember] public int IdCita { get; set; }

        [Required(ErrorMessage = "Debe indicar el resultado de la consulta.")]
        [DataMember] public string ResultadoConsulta { get; set; } = string.Empty;

        [DataMember] public bool RequiereControl { get; set; }
        [DataMember] public DateTime? FechaControl { get; set; }
        [DataMember] public string? DetallesControl { get; set; }
        [DataMember] public int? IdTipoHabitacionRequeridoControl { get; set; }

        // Si el médico marca que la cita requiere control, la fecha y los detalles
        // del control pasan a ser obligatorios (antes se podían dejar vacíos).
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RequiereControl)
            {
                if (!FechaControl.HasValue)
                {
                    yield return new ValidationResult(
                        "Debe indicar la fecha de la cita de control.",
                        new[] { nameof(FechaControl) });
                }
                else if (FechaControl.Value <= DateTime.Now)
                {
                    yield return new ValidationResult(
                        "La fecha de control debe ser posterior a la fecha actual.",
                        new[] { nameof(FechaControl) });
                }

                if (string.IsNullOrWhiteSpace(DetallesControl))
                {
                    yield return new ValidationResult(
                        "Debe indicar los detalles/indicaciones del control.",
                        new[] { nameof(DetallesControl) });
                }
            }
        }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class RespuestaCita
    {
        [DataMember] public bool Ok { get; set; }
        [DataMember] public string Mensaje { get; set; } = string.Empty;
        [DataMember] public int IdGenerado { get; set; }
        [DataMember] public int IdCitaControl { get; set; }
        [DataMember] public int CodigoDisponibilidad { get; set; }
        [DataMember] public string? HoraSugerida { get; set; }
        [DataMember] public int CitasReagendadas { get; set; }
    }
}