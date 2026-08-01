using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IExamenMedicoService
    {
        [OperationContract]
        List<ExamenMedicoDto> Listar();

        [OperationContract]
        List<ExamenMedicoDto> ListarPorPaciente(int idPaciente);

        [OperationContract]
        ExamenMedicoDto? ObtenerPorId(int id);

        [OperationContract]
        List<TipoExamenDto> ListarTiposExamen();

        [OperationContract]
        List<EstadoExamenDto> ListarEstadosExamen();

        [OperationContract]
        List<PacienteExamenDto> ListarPacientes();

        [OperationContract]
        List<MedicoExamenDto> ListarMedicos();

        [OperationContract]
        RespuestaCrud Crear(ExamenMedicoDto examen);

        [OperationContract]
        RespuestaCrud Actualizar(ExamenMedicoDto examen);

        [OperationContract]
        RespuestaCrud Eliminar(int id);
    }
}
