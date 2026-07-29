using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class VentaService : IVentaService
    {
        private readonly IConfiguration _config;
        public VentaService(IConfiguration config) => _config = config;

        public List<VentaDto> Listar() => new VentaRepository(_config).Listar();
        public VentaDto? ObtenerPorId(int id) => new VentaRepository(_config).ObtenerPorId(id);
        public List<ProductoDto> ListarProductos() => new VentaRepository(_config).ListarProductos();

        public RespuestaVenta Registrar(CrearVentaDto dto)
        {
            try
            {
                return new VentaRepository(_config).Registrar(dto);
            }
            catch (Exception ex)
            {
                return new RespuestaVenta { Ok = false, Mensaje = "Error al registrar la venta: " + ex.Message };
            }
        }
    }
}