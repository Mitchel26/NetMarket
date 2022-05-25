using Core.Entities;
using Core.Entities.OrdenCompra;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BusinessLogic.Data
{
    public class MarketDbContext : DbContext
    {
        public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Ejecuta todos las configuraciones de las clases que implementen IEntityTypeConfiguration<>
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Marca> Marcas { get; set; }
        public DbSet<OrdenCompras> OrdenCompras { get; set; }
        public DbSet<OrdenItem> OrdenItems { get; set; }
        public DbSet<TipoEnvio> TipoEnvios { get; set; }

    }
}
