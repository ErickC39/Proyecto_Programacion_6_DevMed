using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DevCCSS.Wcf.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class ExamenMedicoDto
    {
        [DataMember] public int IdExamenMedico { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un paciente.")]
        [DataMember] public int IdPaciente { get; set; }

        [DataMember] public string PacienteIdentificacion { get; set; } = string.Empty;
        [DataMember] public string PacienteNombreCompleto { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un medico.")]
        [DataMember] public int IdEmpleadoMedico { get; set; }

        [DataMember] public string MedicoNombreCompleto { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un tipo de examen.")]
        [DataMember] public int IdTipoExamen { get; set; }

        [DataMember] public string TipoExamen { get; set; } = string.Empty;
        [DataMember] public DateTime FechaSolicitud { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estado.")]
        [DataMember] public int IdEstadoExamen { get; set; }
        [DataMember] public string EstadoExamen { get; set; } = string.Empty;
        [DataMember] public string? Resultado { get; set; }
        [DataMember] public string? Observaciones { get; set; }
        [DataMember] public DateTime? FechaResultado { get; set; }
        [DataMember] public int? IdCitaVinculada { get; set; }

        // Solo se usa al crear: marca la cita programada resultante como
        // prioridad alta / emergencia.
        [DataMember] public bool PrioridadAlta { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class TipoExamenDto
    {
        [DataMember] public int IdTipoExamen { get; set; }
        [DataMember] public string Descripcion { get; set; } = string.Empty;
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class EstadoExamenDto
    {
        [DataMember] public int IdEstadoExamen { get; set; }
        [DataMember] public string Descripcion { get; set; } = string.Empty;
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class PacienteExamenDto
    {
        [DataMember] public int IdPaciente { get; set; }
        [DataMember] public string Identificacion { get; set; } = string.Empty;
        [DataMember] public string NombreCompleto { get; set; } = string.Empty;
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class MedicoExamenDto
    {
        [DataMember] public int IdEmpleado { get; set; }
        [DataMember] public string Identificacion { get; set; } = string.Empty;
        [DataMember] public string NombreCompleto { get; set; } = string.Empty;
        [DataMember] public string? Especialidad { get; set; }
    }
}
