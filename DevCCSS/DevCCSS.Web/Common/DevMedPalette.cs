namespace DevCCSS.Web.Common
{
    // Unica fuente de verdad para los colores de marca fuera del navegador
    // (reportes PDF/Excel, que no pueden leer wwwroot/css/site.css). Deben
    // coincidir siempre con las variables --bs-primary/--bs-secondary/etc.
    // definidas en site.css; si la paleta cambia, se actualiza aqui tambien.
    public static class DevMedPalette
    {
        public const string Primario = "#0F4A68";
        public const string Secundario = "#1F7F85";
        public const string PrimarioSubtle = "#D6E4EA";
    }
}
