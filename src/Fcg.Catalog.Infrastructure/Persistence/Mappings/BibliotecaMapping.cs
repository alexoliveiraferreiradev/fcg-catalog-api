using Fcg.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Catalog.Infrastructure.Persistence.Mappings
{
    public class BibliotecaMapping : IEntityTypeConfiguration<Biblioteca>
    {
        public void Configure(EntityTypeBuilder<Biblioteca> builder)
        {
            builder.ToTable("Bibliotecas");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ValueGeneratedNever();

            builder.Property(b => b.UsuarioId)
                .IsRequired();

            builder.Property(b => b.JogoId)
                .IsRequired();

            builder.Property(b => b.Ativo)
                .IsRequired();

            builder.Property(b => b.DataCadastro)
                .IsRequired();

            builder.Property(b => b.DataAlteracao)
                .IsRequired();

            
            builder.HasOne(b => b.Jogo)
                .WithMany()
                .HasForeignKey(b => b.JogoId)
                .OnDelete(DeleteBehavior.Restrict); 

            
            builder.HasIndex(b => new { b.UsuarioId, b.JogoId })
                .IsUnique();

            
            builder.HasIndex(b => b.UsuarioId);
            builder.HasIndex(b => b.JogoId);
        }
    }
}
