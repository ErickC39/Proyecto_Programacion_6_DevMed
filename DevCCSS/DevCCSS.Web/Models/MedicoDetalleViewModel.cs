using DevCCSS.Web.Contracts;

namespace DevCCSS.Web.Models
{
    public class MedicoDetalleViewModel
    {
        public MedicoDto Medico { get; set; } = new();

        public List<HorarioMedicoDto> Horarios { get; set; } = new();

        public List<CitaDto> Citas { get; set; } = new();
    }
}
