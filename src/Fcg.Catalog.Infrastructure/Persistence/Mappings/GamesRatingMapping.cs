using Fcg.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Catalog.Infrastructure.Persistence.Mappings
{
    public class GamesRatingMapping : IEntityTypeConfiguration<GamesRating>
    {
        public void Configure(EntityTypeBuilder<GamesRating> builder)
        {
            builder.ToTable("GamesRatings");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedNever();

            builder.Property(r => r.GameId)
                .IsRequired();

            builder.Property(r => r.UserId)
                .IsRequired();

            builder.OwnsOne(r => r.Rating, rating =>
            {
                rating.Property(v => v.Value)
                    .HasColumnName("Rating")
                    .IsRequired();
            });
            builder.Navigation(r => r.Rating).IsRequired();

            builder.Property(r => r.RatedAt)
                .IsRequired();

            builder.HasOne<Game>()
                .WithMany()
                .HasForeignKey(r => r.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // Uma nota por usuário em cada jogo.
            builder.HasIndex(r => new { r.GameId, r.UserId })
                .IsUnique();
        }
    }
}
