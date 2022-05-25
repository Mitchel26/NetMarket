using Core.Entities.OrdenCompra;

namespace Core.Interfaces
{
    public interface IOrdenCompraService
    {
        Task<OrdenCompras> AddOrdenCompraAsync(string compradorEmail, int tipoEnvio, string carritoId, Direccion direccion);
        Task<IReadOnlyList<OrdenCompras>> GetOrdenComprasByUserEmailAsync(string email);
        Task<OrdenCompras> GetOrdenComprasByIdAsync(int id, string email);
        Task<IReadOnlyList<TipoEnvio>> GetTipoEnvios();
    }
}
