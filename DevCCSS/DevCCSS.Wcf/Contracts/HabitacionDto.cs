using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DevCCSS.Wcf.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class HabitacionDto
    {
        [DataMember] public int IdHabitacion { get; set; }

        [Required(ErrorMessage = "El numero de habitacion es obligatorio.")]
        [DataMember] public string NumeroHabitacion { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de habitacion.")]
        [DataMember] public int IdTipoHabitacion { get; set; }

        [DataMember] public string TipoHabitacion { get; set; } = string.Empty;
        [DataMember] public int IdEstadoHabitacion { get; set; }
        [DataMember] public string EstadoHabitacion { get; set; } = string.Empty;
        [DataMember] public int? IdPaciente { get; set; }
        [DataMember] public string? PacienteIdentificacion { get; set; }
        [DataMember] public string? PacienteNombreCompleto { get; set; }
        [DataMember] public DateTime? FechaIngreso { get; set; }
        [DataMember] public DateTime? FechaSalida { get; set; }
        [DataMember] public int? IdEmpleadoResponsable { get; set; }
        [DataMember] public string? ResponsableNombreCompleto { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class TipoHabitacionDto
    {
        [DataMember] public int IdTipoHabitacion { get; set; }
        [DataMember] public string Descripcion { get; set; } = string.Empty;
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class EstadoHabitacionDto
    {
        [DataMember] public int IdEstadoHabitacion { get; set; }
        [DataMember] public string Descripcion { get; set; } = string.Empty;
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class PacienteHabitacionDto
    {
        [DataMember] public int IdPaciente { get; set; }
        [DataMember] public string Identificacion { get; set; } = string.Empty;
        [DataMember] public string NombreCompleto { get; set; } = string.Empty;
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class EmpleadoHabitacionDto
    {
        [DataMember] public int IdEmpleado { get; set; }
        [DataMember] public string NombreCompleto { get; set; } = string.Empty;
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class AsignarHabitacionDto
    {
        [DataMember] public int IdHabitacion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un paciente.")]
        [DataMember] public int IdPaciente { get; set; }

        [DataMember] public DateTime FechaIngreso { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un responsable.")]
        [DataMember] public int IdEmpleadoResponsable { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class LiberarHabitacionDto
    {
        [DataMember] public int IdHabitacion { get; set; }
        [DataMember] public DateTime FechaSalida { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un responsable.")]
        [DataMember] public int IdEmpleadoResponsable { get; set; }
    }
}
