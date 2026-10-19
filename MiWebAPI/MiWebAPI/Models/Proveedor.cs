namespace MiWebAPI.Models
{
    public class Proveedor
    {
        public long Id { get; set; }
        public string? Nombre { get; set; }
        public string? ApellidoPaterno { get; set; }
        public string? ApellidoMaterno { get; set; }
        public string? Rfc { get; set; }
        public string? Correo { get; set; }
        public decimal Credito { get; set; }
        public string? NombreBanco { get; set; }
        public DateTime? FechaUltimaModificacion { get; set; }
    }
}