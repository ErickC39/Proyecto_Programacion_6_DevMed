using System.Runtime.Serialization;

namespace DevCCSS.Wcf.Contracts
{
    [DataContract(Namespace = "http://devccss/contracts")]
    public class BitacoraAuditoriaDto
    {
        [DataMember] public int IdAuditoria { get; set; }
        [DataMember] public DateTime Fecha { get; set; }
        [DataMember] public int? IdUsuario { get; set; }
        [DataMember] public string? UsuarioResponsable { get; set; }
        [DataMember] public string Accion { get; set; } = string.Empty;
        [DataMember] public string TablaAfectada { get; set; } = string.Empty;
        [DataMember] public string? DetalleRegistroAntiguo { get; set; }
        [DataMember] public string? DetalleRegistroNuevo { get; set; }
        [DataMember] public string? DireccionIP { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class BitacoraErrorDto
    {
        [DataMember] public int IdError { get; set; }
        [DataMember] public DateTime Fecha { get; set; }
        [DataMember] public string Procedimiento_Trigger { get; set; } = string.Empty;
        [DataMember] public int? NumeroError { get; set; }
        [DataMember] public string MensajeError { get; set; } = string.Empty;
        [DataMember] public int? LineaError { get; set; }
    }

    [DataContract(Namespace = "http://devccss/contracts")]
    public class BitacoraNotificacionDto
    {
        [DataMember] public int IdNotificacion { get; set; }
        [DataMember] public DateTime Fecha { get; set; }
        [DataMember] public int? IdCitaAfectada { get; set; }
        [DataMember] public int? IdCitaEmergencia { get; set; }
        [DataMember] public string? PacienteAfectado { get; set; }
        [DataMember] public string Mensaje { get; set; } = string.Empty;
    }

}
