using DevCCSS.Wcf.Contracts;
using Microsoft.Data.SqlClient;

namespace DevCCSS.Wcf.Models
{
    public class EspecialidadRepository
    {
        private readonly string _connectionString;

        public EspecialidadRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontró la conexión.");
        }

        public List<EspecialidadDto> Listar()
        {
            var lista = new List<EspecialidadDto>();

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"SELECT IdEspecialidad, Nombre
                                FROM Especialidades
                                ORDER BY Nombre";

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                lista.Add(new EspecialidadDto
                {
                    IdEspecialidad = dr.GetInt32(0),
                    Nombre = dr.GetString(1)
                });
            }

            return lista;
        }
    }
}
