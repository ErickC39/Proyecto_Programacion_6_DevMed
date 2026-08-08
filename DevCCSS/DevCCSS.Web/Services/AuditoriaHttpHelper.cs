using System.Security.Claims;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace DevCCSS.Web.Services
{
    // Adjunta el IdUsuario de la sesion web actual como encabezado HTTP en cada
    // llamada WCF, para que el lado Wcf pueda establecerlo como SESSION_CONTEXT
    // y los triggers de auditoria (Bitacora_Auditoria) sepan quien hizo el cambio.
    public static class AuditoriaHttpHelper
    {
        public const string HeaderIdUsuario = "X-IdUsuario";

        public static IDisposable AplicarUsuarioActual(IContextChannel channel, IHttpContextAccessor httpContextAccessor)
        {
            var scope = new OperationContextScope(channel);

            var idUsuario = httpContextAccessor.HttpContext?.User?
                .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";

            var httpProp = new HttpRequestMessageProperty();
            httpProp.Headers[HeaderIdUsuario] = idUsuario;
            OperationContext.Current!.OutgoingMessageProperties[HttpRequestMessageProperty.Name] = httpProp;

            return scope;
        }
    }
}
