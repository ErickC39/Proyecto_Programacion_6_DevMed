using System.Runtime.Serialization;

namespace DevCCSS.Wcf.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class HorarioMedicoDto
    {
        [DataMember] public int IdHorario { get; set; }
        [DataMember] public int IdMedico { get; set; }
        [DataMember] public int DiaSemana { get; set; }      // 1=Lunes ... 7=Domingo
        [DataMember] public string? NombreDia { get; set; }  // solo lectura, para mostrar
        [DataMember] public TimeSpan HoraInicio { get; set; }
        [DataMember] public TimeSpan HoraFin { get; set; }
    }
}
