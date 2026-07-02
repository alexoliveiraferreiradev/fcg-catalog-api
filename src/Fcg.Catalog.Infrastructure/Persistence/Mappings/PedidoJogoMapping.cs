using Fcg.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Catalog.Infrastructure.Persistence.Mappings
{
    public class PedidoJogoMapping : IEntityTypeConfiguration<PedidoJogo>
    {
        public void Configure(EntityTypeBuilder<PedidoJogo> builder)
        {
            builder.ToTable("PedidosJogo");

            builder.HasKey(pj => pj.Id);

            builder.Property(pj => pj.PedidoId)
                .IsRequired();

            builder.Property(pj => pj.JogoId)
                .IsRequired();

            builder.Property(pj => pj.NomeJogo)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(pj => pj.ValorJogo)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        }
    }
}
