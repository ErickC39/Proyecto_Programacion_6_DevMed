using System.Runtime.Serialization;

namespace DevCCSS.Wcf.Contracts
{
    // Respuesta generica para Crear/Actualizar/Eliminar.
    // Asi el MVC sabe si salio bien y muestra el mensaje.
    [DataContract(Namespace = "http://devccss/contracts")]
    public class RespuestaCrud
    {
        [DataMember] public bool Ok { get; set; }
        [DataMember] public string Mensaje { get; set; } = string.Empty;
        [DataMember] public int IdGenerado { get; set; }  // util cuando se crea un registro nuevo
    }
}
