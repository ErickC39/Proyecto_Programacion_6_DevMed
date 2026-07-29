using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface ICitaService
    {
        [OperationContract]
        List<CitaDto> Listar();

        [OperationContract]
        CitaDto? ObtenerPorId(int id);

        [OperationContract]
        RespuestaCita Agendar(AgendarCitaDto dto);

        [OperationContract]
        RespuestaCita AgendarEmergencia(AgendarEmergenciaDto dto);

        [OperationContract]
        RespuestaCita RegistrarLlegada(int idCita);

        [OperationContract]
        RespuestaCita IniciarAtencion(int idCita);

        [OperationContract]
        RespuestaCita Eliminar(int idCita);

        [OperationContract]
        RespuestaCita Cancelar(int idCita);

        [OperationContract]
        RespuestaCita Finalizar(FinalizarCitaDto dto);
    }
}
