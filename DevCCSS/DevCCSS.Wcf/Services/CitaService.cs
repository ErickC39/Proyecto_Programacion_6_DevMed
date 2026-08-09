using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class CitaService : ICitaService
    {
        private readonly CitaRepository _repo;

        public CitaService(CitaRepository repo)
        {
            _repo = repo;
        }

        public List<CitaDto> Listar()
        {
            return _repo.Listar();
        }

        public CitaDto? ObtenerPorId(int id)
        {
            return _repo.ObtenerPorId(id);
        }

        public RespuestaCita Agendar(AgendarCitaDto dto)
        {
            try
            {
                return _repo.Agendar(dto);
            }
            catch (Exception ex)
            {
                return new RespuestaCita { Ok = false, Mensaje = "Error al agendar: " + ex.Message };
            }
        }

        public RespuestaCita AgendarEmergencia(AgendarEmergenciaDto dto)
        {
            try
            {
                return _repo.AgendarEmergencia(dto);
            }
            catch (Exception ex)
            {
                return new RespuestaCita { Ok = false, Mensaje = "Error al agendar la cita de emergencia: " + ex.Message };
            }
        }

        public RespuestaCita RegistrarLlegada(int idCita)
        {
            try
            {
                return _repo.RegistrarLlegada(idCita);
            }
            catch (Exception ex)
            {
                return new RespuestaCita { Ok = false, Mensaje = "Error al registrar llegada: " + ex.Message };
            }
        }

        public RespuestaCita IniciarAtencion(int idCita, int? idHabitacion)
        {
            try
            {
                return _repo.IniciarAtencion(idCita, idHabitacion);
            }
            catch (Exception ex)
            {
                return new RespuestaCita { Ok = false, Mensaje = "Error al iniciar atención: " + ex.Message };
            }
        }

        public RespuestaCita Cancelar(int idCita)
        {
            try
            {
                return _repo.Cancelar(idCita);
            }
            catch (Exception ex)
            {
                return new RespuestaCita { Ok = false, Mensaje = "Error al cancelar la cita: " + ex.Message };
            }
        }

        public RespuestaCita Eliminar(int idCita)
        {
            try
            {
                return _repo.Eliminar(idCita);
            }
            catch (Microsoft.Data.SqlClient.SqlException ex) when (ex.Number == 547)
            {
                return new RespuestaCita
                {
                    Ok = false,
                    Mensaje = "No se puede eliminar esta cita porque tiene notificaciones o una cita de control asociada."
                };
            }
            catch (Exception ex)
            {
                return new RespuestaCita { Ok = false, Mensaje = "Error al eliminar la cita: " + ex.Message };
            }
        }

        public RespuestaCita Finalizar(FinalizarCitaDto dto)
        {
            try
            {
                return _repo.Finalizar(dto);
            }
            catch (Exception ex)
            {
                return new RespuestaCita { Ok = false, Mensaje = "Error al finalizar cita: " + ex.Message };
            }
        }
    }
}
