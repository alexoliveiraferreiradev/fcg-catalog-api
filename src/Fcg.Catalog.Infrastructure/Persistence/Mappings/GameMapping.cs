using Fcg.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Catalog.Infrastructure.Persistence.Mappings
{
    public class JogoMapping : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.ToTable("Games");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Id)
                .ValueGeneratedNever();

            
            builder.OwnsOne(j => j.Name, nameBuilder =>
            {
                nameBuilder.Property(n => n.Value)
                    .HasColumnName("Name")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.OwnsOne(j => j.Description, desc =>
            {
                desc.Property(d => d.Value)
                    .HasColumnName("Description")
                    .HasMaxLength(500)
                    .IsRequired();
            });

            builder.OwnsOne(j => j.BasePrice, priceBuilder =>
            {
                priceBuilder.Property(p => p.Amount)
                    .HasColumnName("BasePrice")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            builder.Property(j => j.IsActive)
                .IsRequired();

            builder.Property(j => j.Genre)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(j => j.CreatedAt)
                .IsRequired();

            builder.Property(j => j.UpdatedAt)
                .IsRequired();

           
            builder.HasMany(j => j.Promotions)
                .WithOne()
                .HasForeignKey(p => p.GameId)
                .OnDelete(DeleteBehavior.Cascade);

           
            builder.Navigation(j => j.Promotions)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

           
            builder.HasIndex(j => j.IsActive);
        }
    }
}
