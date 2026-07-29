using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IMedicamentoService
    {
        [OperationContract]
        List<MedicamentoDto> Listar();

        [OperationContract]
        MedicamentoDto? ObtenerPorId(int id);

        [OperationContract]
        RespuestaCrud Crear(MedicamentoDto medicamento);

        [OperationContract]
        RespuestaCrud Actualizar(MedicamentoDto medicamento);

        [OperationContract]
        RespuestaCrud Eliminar(int id);
    }
}