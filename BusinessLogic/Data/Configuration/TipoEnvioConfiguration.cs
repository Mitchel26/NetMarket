using Core.Entities.OrdenCompra;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BusinessLogic.Data.Configuration
{
    public class TipoEnvioConfiguration : IEntityTypeConfiguration<TipoEnvio>
    {
        public void Configure(EntityTypeBuilder<TipoEnvio> builder)
        {
            builder.Property(t => t.Precio).HasColumnType("decimal(18,2)");
        }
    }
}
