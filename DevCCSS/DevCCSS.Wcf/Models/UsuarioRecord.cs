namespace DevCCSS.Wcf.Models
{
    // Representa la fila de la tabla Usuarios tal cual viene de la BD.
    public class UsuarioRecord
    {
        public int IdUsuario { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        public bool Activo { get; set; }
        public int IntentosFallidos { get; set; }
        public DateTime? BloqueadoHasta { get; set; }
    }
}
