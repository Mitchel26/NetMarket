using Core.Entities.OrdenCompra;

namespace Core.Entities
{
    public class CarritoCompra
    {
        public CarritoCompra()
        {

        }
        public CarritoCompra(string id)
        {
            Id = Id;
        }
        public string Id { get; set; }
        public List<CarritoItem> Items { get; set; } = new List<CarritoItem>();

    }
}
