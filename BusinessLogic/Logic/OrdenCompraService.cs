using Core.Entities;
using Core.Entities.OrdenCompra;
using Core.Interfaces;
using Core.Specifications;

namespace BusinessLogic.Logic
{
    public class OrdenCompraService : IOrdenCompraService
    {
        private readonly ICarritoCompraRepository carritoCompraRepository;
        private readonly IUnitOfWork unitOfWork;

        public OrdenCompraService(ICarritoCompraRepository carritoCompraRepository, IUnitOfWork unitOfWork)

        {
            this.carritoCompraRepository = carritoCompraRepository;
            this.unitOfWork = unitOfWork;
        }
        public async Task<OrdenCompras> AddOrdenCompraAsync(string compradorEmail, int tipoEnvio, string carritoId, Core.Entities.OrdenCompra.Direccion direccion)
        {
            var carritoCompra = await carritoCompraRepository.GetCarritoCompraAsync(carritoId);
            var items = new List<OrdenItem>();
            foreach (var item in carritoCompra.Items)
            {
                var productoItems = await unitOfWork.Repository<Producto>().GetByIdAsync(item.Id);
                var itemOrdenado = new ProductoItemOrdenado(productoItems.Id, productoItems.Nombre, productoItems.Imagen);
                var ordenItems = new OrdenItem(itemOrdenado, productoItems.Precio, item.Cantidad);
                items.Add(ordenItems);
            }

            var tipoEnvioEntity = await unitOfWork.Repository<TipoEnvio>().GetByIdAsync(tipoEnvio);
            var subtotal = items.Sum(item => item.Precio * item.Cantidad);

            var ordenCompra = new OrdenCompras(compradorEmail, direccion, tipoEnvioEntity, items, subtotal);

            unitOfWork.Repository<OrdenCompras>().AddEntity(ordenCompra);
            var resultado = await unitOfWork.Complete();
            if (resultado <= 0)
            {
                return null;
            }

            await carritoCompraRepository.DeleteCarritoCompraAsync(carritoId);

            return ordenCompra;

        }

        public async Task<OrdenCompras> GetOrdenComprasByIdAsync(int id, string email)
        {
            var spec = new OrdenCompraWithItemsSpecification(id, email);
            return await unitOfWork.Repository<OrdenCompras>().GetByIdWithspec(spec);
        }

        public async Task<IReadOnlyList<OrdenCompras>> GetOrdenComprasByUserEmailAsync(string email)
        {
            var spec = new OrdenCompraWithItemsSpecification(email);
            return await unitOfWork.Repository<OrdenCompras>().GetAllWithspec(spec);
        }

        public async Task<IReadOnlyList<TipoEnvio>> GetTipoEnvios()
        {
            return await unitOfWork.Repository<TipoEnvio>().GetAllAsync();
        }
    }
}
