using System.Runtime.Serialization;

namespace DevCCSS.Web.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class HorarioMedicoDto
    {
        [DataMember]
        public int IdHorario { get; set; }

        [DataMember]
        public int IdMedico { get; set; }

        [DataMember]
        public int DiaSemana { get; set; }

        [DataMember]
        public TimeSpan HoraInicio { get; set; }

        [DataMember]
        public TimeSpan HoraFin { get; set; }
    }
}
