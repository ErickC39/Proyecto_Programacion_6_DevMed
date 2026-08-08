using Microsoft.Data.SqlClient;

namespace DevCCSS.Wcf.Services
{
    // SQL Server Express no trae SQL Server Agent, asi que la purga periodica
    // de bitacoras se hace aqui, dentro del propio host, en vez de un job de BD.
    // Corre una vez al iniciar y luego cada 24h; en cada pasada borra todo lo
    // que tenga mas de 15 dias, para que el retenido nunca crezca sin limite
    // (evita que la BD/el proyecto se ponga lento o se caiga por memoria).
    public class BitacoraCleanupService : BackgroundService
    {
        private const int DiasRetencion = 15;
        private static readonly TimeSpan IntervaloEntrePasadas = TimeSpan.FromHours(24);

        private readonly IConfiguration _config;
        private readonly ILogger<BitacoraCleanupService> _logger;

        public BitacoraCleanupService(IConfiguration config, ILogger<BitacoraCleanupService> logger)
        {
            _config = config;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    PurgarRegistrosAntiguos();
                }
                catch (Exception ex)
                {
                    // Un fallo aqui (ej. BD momentaneamente inaccesible) no debe
                    // tumbar el host de WCF; se reintenta en la siguiente pasada.
                    _logger.LogError(ex, "No se pudo purgar la bitacora (retencion {Dias} dias).", DiasRetencion);
                }

                await Task.Delay(IntervaloEntrePasadas, stoppingToken);
            }
        }

        private void PurgarRegistrosAntiguos()
        {
            var connectionString = _config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");

            using var conn = new SqlConnection(connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
DELETE FROM dbo.Bitacora_Auditoria WHERE Fecha < DATEADD(DAY, -@Dias, SYSDATETIME());
DELETE FROM dbo.Bitacora_Errores WHERE Fecha < DATEADD(DAY, -@Dias, SYSDATETIME());
DELETE FROM dbo.Bitacora_Notificaciones WHERE Fecha < DATEADD(DAY, -@Dias, SYSDATETIME());";
            cmd.Parameters.AddWithValue("@Dias", DiasRetencion);

            conn.Open();
            int filas = cmd.ExecuteNonQuery();
            if (filas > 0)
                _logger.LogInformation("Purga de bitacora: {Filas} registro(s) con mas de {Dias} dias eliminados.", filas, DiasRetencion);
        }
    }
}
