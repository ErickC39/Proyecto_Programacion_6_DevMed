using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IMedicoService
    {
        [OperationContract]
        List<MedicoDto> Listar();

        [OperationContract]
        MedicoDto? ObtenerPorId(int idMedico);

        [OperationContract]
        RespuestaCrud Crear(MedicoDto medico);

        [OperationContract]
        RespuestaCrud Actualizar(MedicoDto medico);

        [OperationContract]
        RespuestaCrud Eliminar(int idMedico);

        [OperationContract]
        RespuestaCrud AgregarHorario(HorarioMedicoDto horario);

        [OperationContract]
        List<HorarioMedicoDto> ListarHorarios(int idMedico);

        [OperationContract]
        List<CitaDto> ListarCitasAsignadas(int idMedico);
    }
}
