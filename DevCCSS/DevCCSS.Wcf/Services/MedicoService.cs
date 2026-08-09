using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class MedicoService : IMedicoService
    {
        private readonly IConfiguration _config;

        public MedicoService(IConfiguration config)
        {
            _config = config;
        }

        public List<MedicoDto> Listar()
        {
            try
            {
                return new MedicoRepository(_config).Listar();
            }
            catch (Exception ex)
            {
                throw new Exception("Error en MedicoService.Listar: " + ex.Message, ex);
            }
        }

        public MedicoDto? ObtenerPorId(int idMedico)
        {
            return new MedicoRepository(_config)
                .ObtenerPorId(idMedico);
        }

        public RespuestaCrud Crear(MedicoDto medico)
        {
            try
            {
                return new MedicoRepository(_config).Crear(medico);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return new RespuestaCrud { Ok = false, Mensaje = "Debe seleccionar una especialidad valida." };
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }

        public RespuestaCrud Actualizar(MedicoDto medico)
        {
            try
            {
                return new MedicoRepository(_config).Actualizar(medico);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return new RespuestaCrud { Ok = false, Mensaje = "Debe seleccionar una especialidad valida." };
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }

        public RespuestaCrud AgregarHorario(HorarioMedicoDto horario)
        {
            try
            {
                return new MedicoRepository(_config).AgregarHorario(horario);
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }

        public List<HorarioMedicoDto> ListarHorarios(int idMedico)
        {
            return new MedicoRepository(_config)
                .ListarHorarios(idMedico);
        }

        public List<CitaDto> ListarCitasAsignadas(int idMedico)
        {
            return new MedicoRepository(_config)
              .ListarCitasAsignadas(idMedico);
        }

        public RespuestaCrud Eliminar(int idMedico)
        {
            try
            {
                return new MedicoRepository(_config).Eliminar(idMedico);
            }
            catch (Exception ex)
            {
                return new RespuestaCrud { Ok = false, Mensaje = ex.Message };
            }
        }
    }
}
