using DevCCSS.Wcf.Contracts;
using DevCCSS.Wcf.Models;

namespace DevCCSS.Wcf.Services
{
    public class InventarioService : IInventarioService
    {
        private readonly IConfiguration _config;
        public InventarioService(IConfiguration config) { _config = config; }

        public List<ProductoDto> Listar() => new InventarioRepository(_config).Listar();
        public ProductoDto? ObtenerPorId(int id) => new InventarioRepository(_config).ObtenerPorId(id);

        public RespuestaCrud Crear(ProductoDto producto)
        {
            try { return new InventarioRepository(_config).Crear(producto); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
        public RespuestaCrud Actualizar(ProductoDto producto)
        {
            try { return new InventarioRepository(_config).Actualizar(producto); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
        public RespuestaCrud Eliminar(int id)
        {
            try { return new InventarioRepository(_config).Eliminar(id); }
            catch (Exception ex) { return new RespuestaCrud { Ok = false, Mensaje = ex.Message }; }
        }
    }
}
