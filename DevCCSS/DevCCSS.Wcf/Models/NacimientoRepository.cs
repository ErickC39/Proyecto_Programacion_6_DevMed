using DevCCSS.Wcf.Contracts;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DevCCSS.Wcf.Models
{
    public class NacimientoRepository
    {
        private readonly string _connectionString;

        public NacimientoRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("No se encontro DefaultConnection en appsettings.json");
        }

        public List<NacimientoDto> Listar()
        {
            var lista = new List<NacimientoDto>();
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM dbo.vw_RecienNacidos ORDER BY FechaRegistro DESC;";
            conn.Open();
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                lista.Add(Map(reader));
            return lista;
        }

        public RespuestaCrud Registrar(NacimientoDto n)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Nacimiento_Registrar";

            cmd.Parameters.Add(new SqlParameter("@Identificacion", SqlDbType.NVarChar, 50) { Value = n.Identificacion });
            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = n.Nombre });
            cmd.Parameters.Add(new SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) { Value = n.Apellidos });
            cmd.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = n.FechaNacimiento });
            cmd.Parameters.Add(new SqlParameter("@IdSexoBiologico", SqlDbType.Int) { Value = (object?)n.IdSexoBiologico ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IdTipoSangre", SqlDbType.Int) { Value = (object?)n.IdTipoSangre ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Peso", SqlDbType.Decimal) { Value = (object?)n.Peso ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Estatura", SqlDbType.Decimal) { Value = (object?)n.Estatura ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Alergias", SqlDbType.VarChar) { Value = (object?)n.Alergias ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AntecedentesMedicos", SqlDbType.NVarChar) { Value = (object?)n.AntecedentesMedicos ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IdMadre", SqlDbType.Int) { Value = (object?)n.IdMadre ?? DBNull.Value });

            var pId = new SqlParameter("@IdGenerado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pId);

            conn.Open();
            cmd.ExecuteNonQuery();

            return new RespuestaCrud
            {
                Ok = true,
                Mensaje = "Nacimiento registrado correctamente.",
                IdGenerado = pId.Value == DBNull.Value ? 0 : (int)pId.Value
            };
        }

        public RespuestaCrud RegistrarCompleto(RegistrarNacimientoDto n)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Nacimiento_RegistrarCompleto";

            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = n.Nombre });
            cmd.Parameters.Add(new SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) { Value = n.Apellidos });
            cmd.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = n.FechaNacimiento });
            cmd.Parameters.Add(new SqlParameter("@IdSexoBiologico", SqlDbType.Int) { Value = (object?)n.IdSexoBiologico ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IdTipoSangre", SqlDbType.Int) { Value = (object?)n.IdTipoSangre ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Peso", SqlDbType.Decimal) { Value = (object?)n.Peso ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Estatura", SqlDbType.Decimal) { Value = (object?)n.Estatura ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Alergias", SqlDbType.VarChar) { Value = (object?)n.Alergias ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AntecedentesMedicos", SqlDbType.NVarChar) { Value = (object?)n.AntecedentesMedicos ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IdMadre", SqlDbType.Int) { Value = (object?)n.IdMadre ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PerimetroCefalico", SqlDbType.Decimal) { Value = (object?)n.PerimetroCefalico ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PerimetroToracico", SqlDbType.Decimal) { Value = (object?)n.PerimetroToracico ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Temperatura", SqlDbType.Decimal) { Value = (object?)n.Temperatura ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@FrecuenciaCardiaca", SqlDbType.Int) { Value = (object?)n.FrecuenciaCardiaca ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@FrecuenciaRespiratoria", SqlDbType.Int) { Value = (object?)n.FrecuenciaRespiratoria ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Reflejos", SqlDbType.NVarChar, 255) { Value = (object?)n.Reflejos ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ColoracionPiel", SqlDbType.NVarChar, 50) { Value = (object?)n.ColoracionPiel ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ObservacionesGenerales", SqlDbType.NVarChar) { Value = (object?)n.ObservacionesGenerales ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Apgar", SqlDbType.Structured) { TypeName = "dbo.ApgarType", Value = TablaApgar(n.Apgar) });
            cmd.Parameters.Add(new SqlParameter("@Tamizajes", SqlDbType.Structured) { TypeName = "dbo.TamizajeType", Value = TablaTamizajes(n.Tamizajes) });

            var pId = new SqlParameter("@IdGenerado", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var pIdent = new SqlParameter("@IdentificacionGenerada", SqlDbType.NVarChar, 50) { Direction = ParameterDirection.Output };
            var pMsg = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
            var pCode = new SqlParameter("@CodigoSalida", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pId);
            cmd.Parameters.Add(pIdent);
            cmd.Parameters.Add(pMsg);
            cmd.Parameters.Add(pCode);

            conn.Open();
            cmd.ExecuteNonQuery();

            int codigo = pCode.Value == DBNull.Value ? -1 : (int)pCode.Value;
            return new RespuestaCrud
            {
                Ok = codigo == 0,
                Mensaje = pMsg.Value?.ToString() ?? string.Empty,
                IdGenerado = pId.Value == DBNull.Value ? 0 : (int)pId.Value
            };
        }

        public RespuestaCrud Actualizar(RegistrarNacimientoDto n)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Nacimiento_Actualizar";

            cmd.Parameters.Add(new SqlParameter("@IdPaciente", SqlDbType.Int) { Value = n.IdPaciente });
            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = n.Nombre });
            cmd.Parameters.Add(new SqlParameter("@Apellidos", SqlDbType.NVarChar, 100) { Value = n.Apellidos });
            cmd.Parameters.Add(new SqlParameter("@FechaNacimiento", SqlDbType.Date) { Value = n.FechaNacimiento });
            cmd.Parameters.Add(new SqlParameter("@IdSexoBiologico", SqlDbType.Int) { Value = (object?)n.IdSexoBiologico ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IdTipoSangre", SqlDbType.Int) { Value = (object?)n.IdTipoSangre ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Peso", SqlDbType.Decimal) { Value = (object?)n.Peso ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Estatura", SqlDbType.Decimal) { Value = (object?)n.Estatura ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Alergias", SqlDbType.VarChar) { Value = (object?)n.Alergias ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@AntecedentesMedicos", SqlDbType.NVarChar) { Value = (object?)n.AntecedentesMedicos ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@IdMadre", SqlDbType.Int) { Value = (object?)n.IdMadre ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PerimetroCefalico", SqlDbType.Decimal) { Value = (object?)n.PerimetroCefalico ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@PerimetroToracico", SqlDbType.Decimal) { Value = (object?)n.PerimetroToracico ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Temperatura", SqlDbType.Decimal) { Value = (object?)n.Temperatura ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@FrecuenciaCardiaca", SqlDbType.Int) { Value = (object?)n.FrecuenciaCardiaca ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@FrecuenciaRespiratoria", SqlDbType.Int) { Value = (object?)n.FrecuenciaRespiratoria ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Reflejos", SqlDbType.NVarChar, 255) { Value = (object?)n.Reflejos ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ColoracionPiel", SqlDbType.NVarChar, 50) { Value = (object?)n.ColoracionPiel ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@ObservacionesGenerales", SqlDbType.NVarChar) { Value = (object?)n.ObservacionesGenerales ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Apgar", SqlDbType.Structured) { TypeName = "dbo.ApgarType", Value = TablaApgar(n.Apgar) });
            cmd.Parameters.Add(new SqlParameter("@Tamizajes", SqlDbType.Structured) { TypeName = "dbo.TamizajeType", Value = TablaTamizajes(n.Tamizajes) });

            var pMsg = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
            var pCode = new SqlParameter("@CodigoSalida", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pMsg);
            cmd.Parameters.Add(pCode);

            conn.Open();
            cmd.ExecuteNonQuery();
            int codigo = pCode.Value == DBNull.Value ? -1 : (int)pCode.Value;
            return new RespuestaCrud { Ok = codigo == 0, Mensaje = pMsg.Value?.ToString() ?? string.Empty };
        }

        public RespuestaCrud Eliminar(int idPaciente)
        {
            using var conn = new SqlConnection(_connectionString);
            using var cmd = conn.CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "dbo.sp_Nacimiento_Eliminar";
            cmd.Parameters.Add(new SqlParameter("@IdPaciente", SqlDbType.Int) { Value = idPaciente });
            var pMsg = new SqlParameter("@MensajeSalida", SqlDbType.NVarChar, 500) { Direction = ParameterDirection.Output };
            var pCode = new SqlParameter("@CodigoSalida", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(pMsg);
            cmd.Parameters.Add(pCode);

            conn.Open();
            cmd.ExecuteNonQuery();
            int codigo = pCode.Value == DBNull.Value ? -1 : (int)pCode.Value;
            return new RespuestaCrud { Ok = codigo == 0, Mensaje = pMsg.Value?.ToString() ?? string.Empty };
        }

        private static DataTable TablaApgar(List<ApgarDto> apgar)
        {
            var t = new DataTable();
            t.Columns.Add("Minuto", typeof(int));
            t.Columns.Add("Apariencia", typeof(int));
            t.Columns.Add("Pulso", typeof(int));
            t.Columns.Add("GestoRespuesta", typeof(int));
            t.Columns.Add("Actividad", typeof(int));
            t.Columns.Add("Respiracion", typeof(int));
            foreach (var a in apgar)
                t.Rows.Add(a.Minuto, a.Apariencia, a.Pulso, a.GestoRespuesta, a.Actividad, a.Respiracion);
            return t;
        }

        private static DataTable TablaTamizajes(List<TamizajeDto> tamizajes)
        {
            var t = new DataTable();
            t.Columns.Add("TipoTamizaje", typeof(string));
            t.Columns.Add("Realizado", typeof(bool));
            t.Columns.Add("Resultado", typeof(string));
            foreach (var x in tamizajes)
                t.Rows.Add(x.TipoTamizaje, x.Realizado, (object?)x.Resultado ?? DBNull.Value);
            return t;
        }

        public NacimientoDetalleDto? ObtenerDetalle(int idPaciente)
        {
            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            NacimientoDto? datos = null;
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM dbo.vw_RecienNacidos WHERE IdPaciente = @Id;";
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = idPaciente });
                using var reader = cmd.ExecuteReader();
                if (reader.Read()) datos = Map(reader);
            }
            if (datos is null) return null;

            var detalle = new NacimientoDetalleDto { Datos = datos };

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM dbo.vw_NacimientoApgar WHERE IdPaciente = @Id ORDER BY Minuto;";
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = idPaciente });
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    detalle.Apgar.Add(new ApgarDto
                    {
                        Minuto = reader.GetInt32(reader.GetOrdinal("Minuto")),
                        Apariencia = reader.GetInt32(reader.GetOrdinal("Apariencia")),
                        Pulso = reader.GetInt32(reader.GetOrdinal("Pulso")),
                        GestoRespuesta = reader.GetInt32(reader.GetOrdinal("GestoRespuesta")),
                        Actividad = reader.GetInt32(reader.GetOrdinal("Actividad")),
                        Respiracion = reader.GetInt32(reader.GetOrdinal("Respiracion")),
                        Total = reader.GetInt32(reader.GetOrdinal("Total"))
                    });
                }
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM dbo.vw_NacimientoTamizajes WHERE IdPaciente = @Id ORDER BY TipoTamizaje;";
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = idPaciente });
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    detalle.Tamizajes.Add(new TamizajeDto
                    {
                        TipoTamizaje = reader.GetString(reader.GetOrdinal("TipoTamizaje")),
                        Realizado = reader.GetBoolean(reader.GetOrdinal("Realizado")),
                        Resultado = reader["Resultado"] as string
                    });
                }
            }

            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM dbo.vw_NacimientoExamenFisico WHERE IdPaciente = @Id;";
                cmd.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int) { Value = idPaciente });
                using var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    detalle.PerimetroCefalico = reader["PerimetroCefalico"] == DBNull.Value ? null : reader.GetDecimal(reader.GetOrdinal("PerimetroCefalico"));
                    detalle.PerimetroToracico = reader["PerimetroToracico"] == DBNull.Value ? null : reader.GetDecimal(reader.GetOrdinal("PerimetroToracico"));
                    detalle.Temperatura = reader["Temperatura"] == DBNull.Value ? null : reader.GetDecimal(reader.GetOrdinal("Temperatura"));
                    detalle.FrecuenciaCardiaca = reader["FrecuenciaCardiaca"] == DBNull.Value ? null : reader.GetInt32(reader.GetOrdinal("FrecuenciaCardiaca"));
                    detalle.FrecuenciaRespiratoria = reader["FrecuenciaRespiratoria"] == DBNull.Value ? null : reader.GetInt32(reader.GetOrdinal("FrecuenciaRespiratoria"));
                    detalle.Reflejos = reader["Reflejos"] as string;
                    detalle.ColoracionPiel = reader["ColoracionPiel"] as string;
                    detalle.ObservacionesGenerales = reader["ObservacionesGenerales"] as string;
                }
            }

            return detalle;
        }

        private static NacimientoDto Map(SqlDataReader r) => new NacimientoDto
        {
            IdPaciente = r.GetInt32(r.GetOrdinal("IdPaciente")),
            Identificacion = r.GetString(r.GetOrdinal("Identificacion")),
            Nombre = r.GetString(r.GetOrdinal("Nombre")),
            Apellidos = r.GetString(r.GetOrdinal("Apellidos")),
            FechaNacimiento = r.GetDateTime(r.GetOrdinal("FechaNacimiento")),
            SexoBiologico = r["SexoBiologico"] as string,
            TipoSangre = r["TipoSangre"] as string,
            Peso = r["Peso"] == DBNull.Value ? null : r.GetDecimal(r.GetOrdinal("Peso")),
            Estatura = r["Estatura"] == DBNull.Value ? null : r.GetDecimal(r.GetOrdinal("Estatura")),
            IdMadre = r["IdMadre"] == DBNull.Value ? null : r.GetInt32(r.GetOrdinal("IdMadre")),
            NombreMadre = r["NombreMadre"] as string,
            Apgar1Min = r["Apgar1Min"] == DBNull.Value ? null : r.GetInt32(r.GetOrdinal("Apgar1Min")),
            Apgar5Min = r["Apgar5Min"] == DBNull.Value ? null : r.GetInt32(r.GetOrdinal("Apgar5Min"))
        };
    }
}