using MiWebAPI.Models;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;

namespace MiWebAPI.Data
{
    // Capa de datos: cada método ejecuta un procedimiento almacenado.
    // Los errores de SQL (número duplicado, sueldo <= 0) NO se esconden aquí:
    // suben al controlador para que regrese un mensaje claro.
    public class EmpleadoPracticaData
    {
        private readonly string conexion;

        public EmpleadoPracticaData(IConfiguration configuration)
        {
            conexion = configuration.GetConnectionString("CadenaSQLEmpleado")!;
        }

        public async Task<List<EmpleadoPractica>> ConsultarTodos()
        {
            List<EmpleadoPractica> lista = new List<EmpleadoPractica>();

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("dbo.Empleado_ConsultarTodos", con);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        lista.Add(Mapear(reader));
                    }
                }
            }
            return lista;
        }

        // Regresa null si el Id no existe
        public async Task<EmpleadoPractica?> ConsultarPorId(int id)
        {
            EmpleadoPractica? objeto = null;

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("dbo.Empleado_ConsultarPorId", con);
                cmd.Parameters.AddWithValue("@IdEmpleado", id);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        objeto = Mapear(reader);
                    }
                }
            }
            return objeto;
        }

        // Regresa el Id generado
        public async Task<int> Insertar(EmpleadoPractica objeto)
        {
            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.Empleado_Insertar", con);
                cmd.Parameters.AddWithValue("@NumeroEmpleado", objeto.NumeroEmpleado);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@Apellidos", objeto.Apellidos);
                cmd.Parameters.AddWithValue("@Correo", objeto.Correo);
                cmd.Parameters.AddWithValue("@Puesto", objeto.Puesto);
                cmd.Parameters.AddWithValue("@Sueldo", objeto.Sueldo);
                cmd.CommandType = CommandType.StoredProcedure;

                await con.OpenAsync();
                object? id = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(id);
            }
        }

        // Regresa cuántas filas cambió: 1 = ok, 0 = no existe o está dado de baja
        public async Task<int> Actualizar(EmpleadoPractica objeto)
        {
            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.Empleado_Actualizar", con);
                cmd.Parameters.AddWithValue("@IdEmpleado", objeto.IdEmpleado);
                cmd.Parameters.AddWithValue("@NumeroEmpleado", objeto.NumeroEmpleado);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre);
                cmd.Parameters.AddWithValue("@Apellidos", objeto.Apellidos);
                cmd.Parameters.AddWithValue("@Correo", objeto.Correo);
                cmd.Parameters.AddWithValue("@Puesto", objeto.Puesto);
                cmd.Parameters.AddWithValue("@Sueldo", objeto.Sueldo);
                cmd.CommandType = CommandType.StoredProcedure;

                await con.OpenAsync();
                object? filas = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(filas);
            }
        }

        // Baja lógica. Regresa 1 = dado de baja, 0 = no existe o ya estaba de baja
        public async Task<int> Eliminar(int id)
        {
            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.Empleado_Eliminar", con);
                cmd.Parameters.AddWithValue("@IdEmpleado", id);
                cmd.CommandType = CommandType.StoredProcedure;

                await con.OpenAsync();
                object? filas = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(filas);
            }
        }

        // Convierte la fecha de SQL a "19 de octubre de 2026" (español de México).
        // Si viene NULL (nunca se ha actualizado) regresa null.
        private static readonly CultureInfo Mexico = new CultureInfo("es-MX");

        private static string? FormatoFecha(object valor)
        {
            if (valor is DBNull) return null;
            return Convert.ToDateTime(valor).ToString("d 'de' MMMM 'de' yyyy", Mexico);
        }

        private static EmpleadoPractica Mapear(DbDataReader reader)
        {
            return new EmpleadoPractica
            {
                IdEmpleado = Convert.ToInt32(reader["IdEmpleado"]),
                NumeroEmpleado = reader["NumeroEmpleado"].ToString()!,
                Nombre = reader["Nombre"].ToString()!,
                Apellidos = reader["Apellidos"].ToString()!,
                Correo = reader["Correo"].ToString()!,
                Puesto = reader["Puesto"].ToString()!,
                Sueldo = Convert.ToDecimal(reader["Sueldo"]),
                Activo = Convert.ToBoolean(reader["Activo"]),
                FechaRegistro = FormatoFecha(reader["FechaRegistro"]),
                FechaActualizacion = FormatoFecha(reader["FechaActualizacion"])
            };
        }
    }
}