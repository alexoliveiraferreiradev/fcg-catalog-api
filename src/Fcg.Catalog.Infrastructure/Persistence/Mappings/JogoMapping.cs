using Fcg.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Catalog.Infrastructure.Persistence.Mappings
{
    public class JogoMapping : IEntityTypeConfiguration<Jogo>
    {
        public void Configure(EntityTypeBuilder<Jogo> builder)
        {
            builder.ToTable("Jogos");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Id)
                .ValueGeneratedNever();

            
            builder.OwnsOne(j => j.Nome, nome =>
            {
                nome.Property(n => n.Valor)
                    .HasColumnName("Nome")
                    .HasMaxLength(100)
                    .IsRequired();
            });

            builder.OwnsOne(j => j.Descricao, desc =>
            {
                desc.Property(d => d.Valor)
                    .HasColumnName("Descricao")
                    .HasMaxLength(500)
                    .IsRequired();
            });

            builder.OwnsOne(j => j.PrecoBase, preco =>
            {
                preco.Property(p => p.Valor)
                    .HasColumnName("PrecoBase")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            builder.Property(j => j.Ativo)
                .IsRequired();

            builder.Property(j => j.Genero)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(j => j.DataCadastro)
                .IsRequired();

            builder.Property(j => j.DataAlteracao)
                .IsRequired();

           
            builder.HasMany(j => j.Promocoes)
                .WithOne()
                .HasForeignKey(p => p.JogoId)
                .OnDelete(DeleteBehavior.Cascade);

           
            builder.Navigation(j => j.Promocoes)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

           
            builder.HasIndex(j => j.Ativo);
        }
    }
}
