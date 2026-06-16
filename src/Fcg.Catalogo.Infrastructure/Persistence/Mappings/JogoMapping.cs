using Fcg.Catalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Catalogo.Infrastructure.Persistence.Mappings
{
    public class JogoMapping : IEntityTypeConfiguration<Jogo>
    {
        public void Configure(EntityTypeBuilder<Jogo> builder)
        {
            builder.ToTable("Jogos");

            builder.HasKey(j => j.Id);

            builder.Property(j => j.Id)
                .ValueGeneratedNever();

            // Configuração dos Value Objects mapeados como Owned Types
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

            // Mapeamento de um para muitos (Jogo -> Promocoes)
            builder.HasMany(j => j.Promocoes)
                .WithOne()
                .HasForeignKey(p => p.JogoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Acesso explícito ao backing field privado para preservar o encapsulamento do DDD
            builder.Navigation(j => j.Promocoes)
                .UsePropertyAccessMode(PropertyAccessMode.Field);

            // Índice para melhorar a performance de consultas por status ativo
            builder.HasIndex(j => j.Ativo);
        }
    }
}
