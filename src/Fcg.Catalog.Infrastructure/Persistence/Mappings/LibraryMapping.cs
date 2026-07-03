using Fcg.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Catalog.Infrastructure.Persistence.Mappings
{
    public class BibliotecaMapping : IEntityTypeConfiguration<UserLibrary>
    {
        public void Configure(EntityTypeBuilder<UserLibrary> builder)
        {
            builder.ToTable("Libraries");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ValueGeneratedNever();

            builder.Property(b => b.UserId)
                .IsRequired();

            builder.Property(b => b.GameId)
                .IsRequired();

            builder.Property(b => b.IsActive)
                .IsRequired();

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.Property(b => b.UpdatedAt)
                .IsRequired();

            
            builder.HasOne(b => b.Game)
                .WithMany()
                .HasForeignKey(b => b.GameId)
                .OnDelete(DeleteBehavior.Restrict); 

            
            builder.HasIndex(b => new { b.UserId, b.GameId })
                .IsUnique();

            
            builder.HasIndex(b => b.UserId);
            builder.HasIndex(b => b.GameId);
        }
    }
}
