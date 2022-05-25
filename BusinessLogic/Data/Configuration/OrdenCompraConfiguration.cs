using Core.Entities.OrdenCompra;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessLogic.Data.Configuration
{
    public class OrdenCompraConfiguration : IEntityTypeConfiguration<OrdenCompras>
    {
        public void Configure(EntityTypeBuilder<OrdenCompras> builder)
        {
            // Relacion con la entidad Dirección (es dependiente de la entidad OrdenCompras)
            builder.OwnsOne(o => o.DireccionEnvio, x => { x.WithOwner(); });

            builder.Property(o => o.Status).HasConversion(o => o.ToString(), o => (OrdenStatus)Enum.Parse(typeof(OrdenStatus), o));

            // Eliminación en cascada de los OrdenItems, cuando se elimine la OrdenCompras
            builder.HasMany(o => o.OrdenItems).WithOne().OnDelete(DeleteBehavior.Cascade);

            builder.Property(o => o.Subtotal).HasColumnType("decimal(18,2)");
        }
    }
}
