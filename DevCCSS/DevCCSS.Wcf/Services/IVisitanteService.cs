using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IVisitanteService
    {
        [OperationContract]
        List<VisitanteDto> Listar();

        [OperationContract]
        VisitanteDto? ObtenerPorId(int id);

        [OperationContract]
        RespuestaCrud Crear(VisitanteDto visitante);

        [OperationContract]
        RespuestaCrud Actualizar(VisitanteDto visitante);

        [OperationContract]
        RespuestaCrud Eliminar(int id);

        [OperationContract]
        RespuestaCrud RegistrarSalida(int idVisita, DateTime? fechaHoraSalida);
    }
}
