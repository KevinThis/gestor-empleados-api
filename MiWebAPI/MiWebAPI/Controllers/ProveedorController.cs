using Microsoft.AspNetCore.Mvc;
using MiWebAPI.Data;
using MiWebAPI.Models;

namespace MiWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly ProveedorData _proveedorData;
        public ProveedorController(ProveedorData proveedorData)
        {
            _proveedorData = proveedorData;
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<Proveedor> Lista = await _proveedorData.Lista();
            return StatusCode(StatusCodes.Status200OK, Lista);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obtener(long id)
        {
            Proveedor objeto = await _proveedorData.Obtener(id);
            return StatusCode(StatusCodes.Status200OK, objeto);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Proveedor objeto)
        {
            bool respuesta = await _proveedorData.Crear(objeto);
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = respuesta });
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] Proveedor objeto)
        {
            bool respuesta = await _proveedorData.Editar(objeto);
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = respuesta });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            bool respuesta = await _proveedorData.Eliminar(id);
            return StatusCode(StatusCodes.Status200OK, new { isSuccess = respuesta });
        }
    }
}