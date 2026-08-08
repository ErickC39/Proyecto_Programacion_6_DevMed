namespace DevCCSS.Web.Common
{
    // Marca un controlador con el nombre de "Modulo" tal cual aparece en la
    // tabla Roles_Permisos, para que PermisoAuthorizationFilter sepa contra
    // que fila de esa tabla debe validar las acciones de ese controlador.
    // Un controlador SIN este atributo no queda sujeto a permisos finos
    // (solo a los [Authorize(Roles=...)] que ya tenga) -- asi los
    // controladores que no aparecen en Roles_Permisos (Home, Account,
    // Medicos, Especialidades) no se ven afectados.
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuloAttribute : Attribute
    {
        public string Nombre { get; }
        public ModuloAttribute(string nombre) => Nombre = nombre;
    }
}
