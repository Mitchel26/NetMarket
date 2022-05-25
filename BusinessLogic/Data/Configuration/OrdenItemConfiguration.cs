using Core.Entities.OrdenCompra;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessLogic.Data.Configuration
{
    public class OrdenItemConfiguration : IEntityTypeConfiguration<OrdenItem>
    {
        public void Configure(EntityTypeBuilder<OrdenItem> builder)
        {
            // Relación con ProductoItemOrdenado
            builder.OwnsOne(oi => oi.ItemOrdenado, pi => { pi.WithOwner(); });

            builder.Property(oi => oi.Precio).HasColumnType("decimal(18,2)");

        }
    }
}
