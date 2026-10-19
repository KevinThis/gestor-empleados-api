using System.ComponentModel.DataAnnotations;

// Practica Kevin Avendaño Ramírez - tabla Empleado (base PRACTICA_EMPLEADO)
// Se llama EmpleadoPractica para no chocar con el Empleado de DDCrudAngular

namespace MiWebAPI.Models
{
    public class EmpleadoPractica
    {
        public int IdEmpleado { get; set; }

        [Required, StringLength(20)]
        public string NumeroEmpleado { get; set; } = "";

        [Required, StringLength(100)]
        public string Nombre { get; set; } = "";

        [Required, StringLength(150)]
        public string Apellidos { get; set; } = "";

        [Required, StringLength(150), EmailAddress]
        public string Correo { get; set; } = "";

        [Required, StringLength(100)]
        public string Puesto { get; set; } = "";

        [Range(typeof(decimal), "0.01", "99999999.99", ErrorMessage = "El sueldo debe ser mayor que cero.")]
        public decimal Sueldo { get; set; }

        // Estos tres los llena la base de datos; si vienen en el JSON se ignoran
        public bool Activo { get; set; }
        // En SQL son DATETIME2; aquí se entregan ya como texto legible,
        // por ejemplo "19 de octubre de 2026"
        public string? FechaRegistro { get; set; }
        public string? FechaActualizacion { get; set; }
    }
}