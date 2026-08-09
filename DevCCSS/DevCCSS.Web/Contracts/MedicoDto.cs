using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class MedicoDto
    {
        [DataMember]
        public int IdMedico { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un empleado.")]
        [DataMember]
        public int IdEmpleado { get; set; }

        [DataMember]
        public string Identificacion { get; set; } = "";

        [DataMember]
        public string Nombre { get; set; } = "";

        [DataMember]
        public string Apellidos { get; set; } = "";

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una especialidad.")]
        [DataMember]
        public int IdEspecialidad { get; set; }

        [DataMember]
        public string Especialidad { get; set; } = "";

        [DataMember]
        public bool Activo { get; set; }

        [DataMember]
        public List<HorarioMedicoDto> Horarios { get; set; } = new();

        [DataMember]
        public List<CitaDto> CitasAsignadas { get; set; } = new();
    }
}
