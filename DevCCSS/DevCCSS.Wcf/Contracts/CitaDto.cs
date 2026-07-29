using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DevCCSS.Wcf.Contracts
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
        // Indica si esta cita fue generada como cita de control/revisión
        // (por ejemplo, a partir de Finalizar con RequiereControl = true).
        [DataMember] public bool EsCitaControl { get; set; }
        // Indica si la cita que el médico tiene "En Progreso" ahora mismo
        // (la que causa la demora de quienes esperan) es una cita de control.
        [DataMember] public bool CitaPreviaEsControl { get; set; }
        // Indica si ESTA cita fue trasladada porque el médico atendió una emergencia.
        [DataMember] public bool FueReagendadaPorEmergencia { get; set; }
        [DataMember] public string? MensajeReagendo { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class AgendarCitaDto
    {
        [Required(ErrorMessage = "Debe ingresar la identificacion del paciente.")]
        [DataMember] public string PacienteIdentificacion { get; set; } = string.Empty;
        [DataMember] public int IdMedico { get; set; }
        [DataMember] public DateTime FechaHoraCita { get; set; }
        [DataMember] public string PrioridadCita { get; set; } = "Normal";
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class AgendarEmergenciaDto
    {
        [Required(ErrorMessage = "Debe indicar el empleado que solicita la atención de emergencia.")]
        [DataMember] public int IdEmpleado { get; set; }
        [DataMember] public int IdMedico { get; set; }
        [DataMember] public DateTime FechaHoraCita { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class FinalizarCitaDto
    {
        [DataMember] public int IdCita { get; set; }
        [DataMember] public string ResultadoConsulta { get; set; } = string.Empty;
        [DataMember] public bool RequiereControl { get; set; }
        [DataMember] public DateTime? FechaControl { get; set; }
        [DataMember] public string? DetallesControl { get; set; }
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
