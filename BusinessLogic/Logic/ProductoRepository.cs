using BusinessLogic.Data;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessLogic.Logic
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly MarketDbContext context;

        public ProductoRepository(MarketDbContext context)
        {
            this.context = context;
        }
        public async Task<Producto> GetProductoByIdAsync(int id)
        {
            return await context.Productos.Include(p => p.Marca).Include(p => p.Categoria).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IReadOnlyList<Producto>> GetProductosAsync()
        {
            return await context.Productos.ToListAsync();
        }
    }
}
