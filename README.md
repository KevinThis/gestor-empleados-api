# gestor-empleados-api

Proyectos de APIs de Gestor de Empleados.

- **Alumno:** Kevin Avendaño Ramírez
- **Actividad:** Tablas, procedimientos almacenados y API con Postman
- **Tabla asignada:** Empleado (NumeroEmpleado, Nombre, Apellidos, Correo, Puesto, Sueldo)

API en .NET 8 que ejecuta procedimientos almacenados de SQL Server. La parte de la actividad vive en estos archivos:

| Archivo | Qué hace |
|---|---|
| `database/PRACTICA_EMPLEADO.sql` | Crea la base, la tabla `Empleado`, los 5 procedimientos y 10 registros de prueba |
| `MiWebAPI/MiWebAPI/Models/EmpleadoPractica.cs` | Modelo de la tabla, con validaciones |
| `MiWebAPI/MiWebAPI/Data/EmpleadoPracticaData.cs` | Ejecuta los procedimientos almacenados |
| `MiWebAPI/MiWebAPI/Controllers/EmpleadosController.cs` | Rutas `/api/empleados` |
| `postman/DEV_EMPLEADO_PRACTICA.postman_collection.json` | Colección de Postman con las 5 operaciones |

> El proyecto también tiene otros controladores de prácticas anteriores (`Empleado`, `Producto`, `Proveedor`). La clase se llama `EmpleadoPractica` para no chocar con el `Empleado` anterior; la tabla en SQL sí se llama `Empleado`.

## 1. Base de datos

1. Abre SQL Server Management Studio.
2. Ejecuta completo `database/PRACTICA_EMPLEADO.sql` (F5).
3. Se crea la base `PRACTICA_EMPLEADO` con:
   - Tabla `Empleado`: `IdEmpleado` autoincrementable, `NumeroEmpleado` obligatorio y único, `Sueldo DECIMAL(10,2)` mayor que cero, `Activo` (1 por defecto), `FechaRegistro` automática y `FechaActualizacion` nula hasta que se modifica o se da de baja.
   - Procedimientos: `Empleado_Insertar`, `Empleado_Actualizar`, `Empleado_Eliminar` (baja lógica), `Empleado_ConsultarPorId`, `Empleado_ConsultarTodos` (solo activos).

## 2. Configurar la conexión

En `MiWebAPI/MiWebAPI/appsettings.json`, la cadena `CadenaSQLEmpleado` apunta a la base de la práctica. Cambia `Data Source` por tu servidor:

```json
"CadenaSQLEmpleado": "Data Source=TU_SERVIDOR\\INSTANCIA;Initial Catalog=PRACTICA_EMPLEADO;Integrated Security=True;TrustServerCertificate=True;"
```

## 3. Ejecutar la API

Abre `MiWebAPI/MiWebAPI.sln` en Visual Studio 2022 y presiona F5. Swagger abre en `http://localhost:5299/swagger`.

## 4. Endpoints

| Operación | Método | Ruta | Respuesta correcta |
|---|---|---|---|
| Registrar | POST | `/api/empleados` | 201 + `idEmpleado` |
| Consultar activos | GET | `/api/empleados` | 200 + lista |
| Consultar por ID | GET | `/api/empleados/{id}` | 200 / 404 si no existe |
| Actualizar | PUT | `/api/empleados/{id}` | 200 / 404 si no existe o está de baja |
| Baja lógica | DELETE | `/api/empleados/{id}` | 200 / 404 si no existe o ya estaba de baja |

Errores de reglas (número duplicado, sueldo ≤ 0, campos vacíos, correo inválido) regresan **400** con un mensaje.

Body para POST y PUT:

```json
{
  "numeroEmpleado": "EMP-0011",
  "nombre": "Kevin",
  "apellidos": "Avendaño Ramírez",
  "correo": "kevin.avendano@empresa.com",
  "puesto": "Desarrollador .NET",
  "sueldo": 22000.00
}
```

## 5. Pruebas en Postman

1. Postman → **Import** → `postman/DEV_EMPLEADO_PRACTICA.postman_collection.json`.
2. La variable `baseUrl` apunta a `http://localhost:5299`.
3. Orden de la demostración: **AddEmpleadoPractica** (guarda el Id nuevo) → **GetEmpleadoPractica** → **GetEmpleadoPracticaById** → **UpdEmpleadoPractica** → **GetEmpleadoPracticaById** (ver el cambio) → **DeleteEmpleadoPractica** → **GetEmpleadoPractica** (ya no aparece).

Si repites **AddEmpleadoPractica** con el mismo `numeroEmpleado` la API responde 400, porque es único; usa otro (`EMP-0012`, etc.).
