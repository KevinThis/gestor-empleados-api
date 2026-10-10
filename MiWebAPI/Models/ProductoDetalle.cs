namespace MiWebAPI.Controllers
{
    public class ProductoDetalle
    {
        public long Id { get; set;  }
        public string? NombreProducto { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Total { get; set; }
        public string? descripcion { get; set; }
    }
}
