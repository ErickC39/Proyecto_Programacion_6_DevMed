using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface INacimientoService
    {
        [OperationContract]
        List<NacimientoDto> Listar();

        [OperationContract]
        RespuestaCrud Registrar(NacimientoDto nacimiento);

        [OperationContract]
        RespuestaCrud RegistrarCompleto(RegistrarNacimientoDto nacimiento);

        [OperationContract]
        NacimientoDetalleDto? ObtenerDetalle(int idPaciente);

        [OperationContract]
        RespuestaCrud Actualizar(RegistrarNacimientoDto nacimiento);

        [OperationContract]
        RespuestaCrud Eliminar(int idPaciente);
    }
}