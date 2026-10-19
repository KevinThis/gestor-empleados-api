using MiWebAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace MiWebAPI.Data
{
    public class ProveedorData
    {

        private readonly string conexion;

        public ProveedorData(IConfiguration configuration)
        {
            // Otra base de datos, otra cadena de conexion (ver appsettings.json)
            conexion = configuration.GetConnectionString("CadenaSQLProveedor")!;
        }

        public async Task<List<Proveedor>> Lista()
        {
            List<Proveedor> lista = new List<Proveedor>();

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("dbo.PROVEEDOR_SELECCIONA", con);
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

        public async Task<Proveedor> Obtener(long id)
        {
            Proveedor objeto = new Proveedor();

            using (var con = new SqlConnection(conexion))
            {
                await con.OpenAsync();
                SqlCommand cmd = new SqlCommand("dbo.PROVEEDOR_SELECCIONA_ID", con);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.CommandType = CommandType.StoredProcedure;

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        objeto = Mapear(reader);
                    }
                }
            }
            return objeto;
        }

        public async Task<bool> Crear(Proveedor objeto)
        {
            bool respuesta = true;

            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.PROVEEDOR_AGREGA", con);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre ?? "");
                cmd.Parameters.AddWithValue("@ApellidoPaterno", objeto.ApellidoPaterno ?? "");
                cmd.Parameters.AddWithValue("@ApellidoMaterno", objeto.ApellidoMaterno ?? "");
                cmd.Parameters.AddWithValue("@Rfc", objeto.Rfc ?? "");
                cmd.Parameters.AddWithValue("@Correo", objeto.Correo ?? "");
                cmd.Parameters.AddWithValue("@Credito", objeto.Credito);
                cmd.Parameters.AddWithValue("@NombreBanco", objeto.NombreBanco ?? "");
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    await con.OpenAsync();
                    respuesta = await cmd.ExecuteNonQueryAsync() > 0 ? true : false;
                }
                catch
                {
                    respuesta = false;
                }
            }
            return respuesta;
        }

        public async Task<bool> Editar(Proveedor objeto)
        {
            bool respuesta = true;

            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.PROVEEDOR_ACTUALIZA", con);
                cmd.Parameters.AddWithValue("@Id", objeto.Id);
                cmd.Parameters.AddWithValue("@Nombre", objeto.Nombre ?? "");
                cmd.Parameters.AddWithValue("@ApellidoPaterno", objeto.ApellidoPaterno ?? "");
                cmd.Parameters.AddWithValue("@ApellidoMaterno", objeto.ApellidoMaterno ?? "");
                cmd.Parameters.AddWithValue("@Rfc", objeto.Rfc ?? "");
                cmd.Parameters.AddWithValue("@Correo", objeto.Correo ?? "");
                cmd.Parameters.AddWithValue("@Credito", objeto.Credito);
                cmd.Parameters.AddWithValue("@NombreBanco", objeto.NombreBanco ?? "");
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    await con.OpenAsync();
                    respuesta = await cmd.ExecuteNonQueryAsync() > 0 ? true : false;
                }
                catch
                {
                    respuesta = false;
                }
            }
            return respuesta;
        }

        public async Task<bool> Eliminar(long id)
        {
            bool respuesta = true;

            using (var con = new SqlConnection(conexion))
            {
                SqlCommand cmd = new SqlCommand("dbo.PROVEEDOR_ELIMINA", con);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.CommandType = CommandType.StoredProcedure;
                try
                {
                    await con.OpenAsync();
                    respuesta = await cmd.ExecuteNonQueryAsync() > 0 ? true : false;
                }
                catch
                {
                    respuesta = false;
                }
            }
            return respuesta;
        }

        // Un solo lugar para leer las columnas; tolera NULL porque la tabla los permite
        private static Proveedor Mapear(System.Data.Common.DbDataReader reader)
        {
            return new Proveedor
            {
                Id = Convert.ToInt64(reader["Id"]),
                Nombre = reader["Nombre"].ToString(),
                ApellidoPaterno = reader["ApellidoPaterno"].ToString(),
                ApellidoMaterno = reader["ApellidoMaterno"].ToString(),
                Rfc = reader["Rfc"].ToString(),
                Correo = reader["Correo"].ToString(),
                Credito = reader["Credito"] is DBNull ? 0 : Convert.ToDecimal(reader["Credito"]),
                NombreBanco = reader["NombreBanco"].ToString(),
                FechaUltimaModificacion = reader["FechaUltimaModificacion"] is DBNull ? null : Convert.ToDateTime(reader["FechaUltimaModificacion"])
            };
        }

    }
}