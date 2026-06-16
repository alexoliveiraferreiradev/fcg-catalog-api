using Fcg.Catalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Catalogo.Infrastructure.Persistence.Mappings
{
    public class PromocaoMapping : IEntityTypeConfiguration<Promocao>
    {
        public void Configure(EntityTypeBuilder<Promocao> builder)
        {
            builder.ToTable("Promocoes");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.JogoId)
                .IsRequired();

            // Mapeando Preco como Owned Type
            builder.OwnsOne(p => p.ValorPromocao, preco =>
            {
                preco.Property(pr => pr.Valor)
                    .HasColumnName("ValorPromocao")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            // Mapeando Periodo (ValueObject) como Owned Type
            builder.OwnsOne(p => p.Periodo, periodo =>
            {
                periodo.Property(pe => pe.DataInicio)
                    .HasColumnName("DataInicio")
                    .IsRequired();

                periodo.Property(pe => pe.DataFim)
                    .HasColumnName("DataFim")
                    .IsRequired();
            });

            builder.Property(p => p.Ativo)
                .IsRequired();

            builder.Property(p => p.DataCadastro)
                .IsRequired();

            builder.Property(p => p.DataAlteracao)
                .IsRequired();

            // Índices para performance em buscas e joins frequentes
            builder.HasIndex(p => p.JogoId);
            builder.HasIndex(p => p.Ativo);
        }
    }
}
