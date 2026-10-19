using MiWebAPI.Data;
using MiWebAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace MiWebAPI.Controllers
{
    [Route("api/empleados")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private readonly EmpleadoPracticaData _empleadoData;

        public EmpleadosController(EmpleadoPracticaData empleadoData)
        {
            _empleadoData = empleadoData;
        }

        // GET /api/empleados  -> solo activos
        [HttpGet]
        public async Task<IActionResult> ConsultarTodos()
        {
            List<EmpleadoPractica> lista = await _empleadoData.ConsultarTodos();
            return StatusCode(StatusCodes.Status200OK, lista);
        }

        // GET /api/empleados/1
        [HttpGet("{id:int}")]
        public async Task<IActionResult> ConsultarPorId(int id)
        {
            EmpleadoPractica? objeto = await _empleadoData.ConsultarPorId(id);
            if (objeto == null)
                return StatusCode(StatusCodes.Status404NotFound, new { isSuccess = false, mensaje = $"No existe el empleado {id}." });

            return StatusCode(StatusCodes.Status200OK, objeto);
        }

        // POST /api/empleados
        [HttpPost]
        public async Task<IActionResult> Insertar([FromBody] EmpleadoPractica objeto)
        {
            try
            {
                int idNuevo = await _empleadoData.Insertar(objeto);
                return StatusCode(StatusCodes.Status201Created, new { isSuccess = true, idEmpleado = idNuevo });
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = false, mensaje = MensajeSql(ex) });
            }
        }

        // PUT /api/empleados/1
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] EmpleadoPractica objeto)
        {
            objeto.IdEmpleado = id;   // el Id manda la ruta, no el body
            try
            {
                int filas = await _empleadoData.Actualizar(objeto);
                if (filas == 0)
                    return StatusCode(StatusCodes.Status404NotFound, new { isSuccess = false, mensaje = $"El empleado {id} no existe o está dado de baja." });

                return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
            }
            catch (SqlException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { isSuccess = false, mensaje = MensajeSql(ex) });
            }
        }

        // DELETE /api/empleados/1  -> baja lógica (Activo = 0)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            int filas = await _empleadoData.Eliminar(id);
            if (filas == 0)
                return StatusCode(StatusCodes.Status404NotFound, new { isSuccess = false, mensaje = $"El empleado {id} no existe o ya estaba dado de baja." });

            return StatusCode(StatusCodes.Status200OK, new { isSuccess = true });
        }

        // Traduce los errores de reglas de la tabla a un mensaje entendible
        private static string MensajeSql(SqlException ex)
        {
            switch (ex.Number)
            {
                case 2627:
                case 2601:
                    return "Ya existe un empleado con ese NumeroEmpleado.";
                case 547:
                    return "El sueldo debe ser mayor que cero.";
                default:
                    return ex.Message;
            }
        }
    }
}