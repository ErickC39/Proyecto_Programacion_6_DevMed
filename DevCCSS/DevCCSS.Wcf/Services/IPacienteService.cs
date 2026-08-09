using CoreWCF;
using DevCCSS.Wcf.Contracts;

namespace DevCCSS.Wcf.Services
{
    [ServiceContract(Namespace = "http://devccss/services")]
    public interface IPacienteService
    {
        [OperationContract]
        List<PacienteDto> Listar();

        [OperationContract]
        PacienteDto? ObtenerPorId(int id);

        [OperationContract]
        RespuestaCrud Crear(PacienteDto paciente);

        [OperationContract]
        RespuestaCrud Actualizar(PacienteDto paciente);

        [OperationContract]
        RespuestaCrud Eliminar(int id);

        [OperationContract]
        RespuestaCrud GuardarExpediente(ExpedienteDto expediente);

        [OperationContract]
        List<TipoSangreDto> ListarTiposSangre();

        [OperationContract]
        List<SexoBiologicoDto> ListarSexosBiologicos();

        [OperationContract]
        List<IdentidadGeneroDto> ListarIdentidadesGenero();

        [OperationContract]
        List<TipoIdentificacionDto> ListarTiposIdentificacion();
    }
}
