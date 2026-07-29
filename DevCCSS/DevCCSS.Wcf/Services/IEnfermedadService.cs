using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IEnfermedadService
    {
        [OperationContract]
        List<EnfermedadDto> Listar();

        [OperationContract]
        EnfermedadDto? ObtenerPorId(int id);

        [OperationContract]
        RespuestaCrud Crear(EnfermedadDto enfermedad);

        [OperationContract]
        RespuestaCrud Actualizar(EnfermedadDto enfermedad);

        [OperationContract]
        RespuestaCrud Eliminar(int id);

        [OperationContract]
        List<TratamientoEnfermedadDto> ListarMedicamentos(int idEnfermedad);

        [OperationContract]
        RespuestaCrud AsignarMedicamento(int idEnfermedad, int idMedicamento, string? observacion);

        [OperationContract]
        RespuestaCrud QuitarMedicamento(int idEnfermedad, int idMedicamento);
    }
}
