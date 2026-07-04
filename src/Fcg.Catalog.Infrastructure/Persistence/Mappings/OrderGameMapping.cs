using Fcg.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Catalog.Infrastructure.Persistence.Mappings
{
    public class OrderGameMapping : IEntityTypeConfiguration<OrderGame>
    {
        public void Configure(EntityTypeBuilder<OrderGame> builder)
        {
            builder.ToTable("OrderGames");

            builder.HasKey(pj => pj.Id);

            builder.Property(pj => pj.OrderId)
                .IsRequired();

            builder.Property(pj => pj.GameId)
                .IsRequired();

            builder.Property(pj => pj.GameName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(pj => pj.GameAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();
        }
    }
}
