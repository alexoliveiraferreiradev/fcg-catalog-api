using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Catalog.Infrastructure.Persistence.Mappings
{
    public class PromocaoMapping : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.ToTable("Promotions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.GameId)
                .IsRequired();

            
            builder.OwnsOne(p => p.ValorPromocao, Price =>
            {
                Price.Property(pr => pr.Amount)
                    .HasColumnName("ValorPromocao")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            
            builder.OwnsOne(p => p.Period, Period =>
            {
                Period.Property(pe => pe.StartDate)
                    .HasColumnName("StartDate")
                    .IsRequired();

                Period.Property(pe => pe.EndDate)
                    .HasColumnName("EndDate")
                    .IsRequired();
            });

            builder.Property(p => p.IsActive)
                .IsRequired();

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired();

           
            builder.HasIndex(p => p.GameId);
            builder.HasIndex(p => p.IsActive);
        }
    }
}
