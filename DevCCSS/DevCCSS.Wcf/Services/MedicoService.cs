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

        public EmpleadoDto? BuscarEmpleado(string identificacion)
        {
            return new MedicoRepository(_config)
                .BuscarEmpleado(identificacion);
        }

        public RespuestaCrud Crear(MedicoDto medico)
        {
            try
            {
                return new MedicoRepository(_config).Crear(medico);
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
    }
}
