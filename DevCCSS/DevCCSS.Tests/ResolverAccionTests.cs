using DevCCSS.Web.Common;
using Xunit;

namespace DevCCSS.Tests
{
    // Cubre la heuristica de PermisoAuthorizationFilter.ResolverAccion, que
    // decide que permiso (Ver/Crear/Editar/Eliminar) de Roles_Permisos aplica
    // segun el nombre de la accion del controlador MVC.
    public class ResolverAccionTests
    {
        [Theory]
        [InlineData("Index", PermisoAuthorizationFilter.Accion.Ver)]
        [InlineData("Details", PermisoAuthorizationFilter.Accion.Ver)]
        [InlineData("Detalle", PermisoAuthorizationFilter.Accion.Ver)]
        [InlineData("Create", PermisoAuthorizationFilter.Accion.Crear)]
        [InlineData("Registrar", PermisoAuthorizationFilter.Accion.Crear)]
        [InlineData("Agendar", PermisoAuthorizationFilter.Accion.Crear)]
        [InlineData("AgendarEmergencia", PermisoAuthorizationFilter.Accion.Crear)]
        [InlineData("Solicitar", PermisoAuthorizationFilter.Accion.Crear)]
        [InlineData("Delete", PermisoAuthorizationFilter.Accion.Eliminar)]
        [InlineData("DeleteConfirmed", PermisoAuthorizationFilter.Accion.Eliminar)]
        [InlineData("Cancelar", PermisoAuthorizationFilter.Accion.Eliminar)]
        [InlineData("Edit", PermisoAuthorizationFilter.Accion.Editar)]
        [InlineData("CambiarEstado", PermisoAuthorizationFilter.Accion.Editar)]
        [InlineData("Asignar", PermisoAuthorizationFilter.Accion.Editar)]
        [InlineData("RegistrarLlegada", PermisoAuthorizationFilter.Accion.Crear)]
        public void ResolverAccion_ClasificaSegunElNombreDeLaAccion(string actionName, PermisoAuthorizationFilter.Accion esperado)
        {
            var resultado = PermisoAuthorizationFilter.ResolverAccion(actionName);

            Assert.Equal(esperado, resultado);
        }

        [Fact]
        public void ResolverAccion_NoDistingueMayusculasMinusculas()
        {
            Assert.Equal(PermisoAuthorizationFilter.Accion.Eliminar, PermisoAuthorizationFilter.ResolverAccion("DELETE"));
            Assert.Equal(PermisoAuthorizationFilter.Accion.Ver, PermisoAuthorizationFilter.ResolverAccion("INDEX"));
        }
    }
}
