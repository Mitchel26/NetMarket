namespace WebApi.DTOs
{
    public class OrdenCompraDTO
    {
        public string  CarritoCompraId { get; set; }
        public int TipoEnvio { get; set; }
        public DireccionDTO DireccionEnvio { get; set; }
    }
}
