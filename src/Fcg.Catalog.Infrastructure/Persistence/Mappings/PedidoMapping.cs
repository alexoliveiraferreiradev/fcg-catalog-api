using Fcg.Catalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Catalog.Infrastructure.Persistence.Mappings
{
    public class PedidoMapping : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.ToTable("Pedidos");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.UsuarioId)
                .IsRequired();

            builder.Property(p => p.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.DataCadastro)
                .IsRequired();

            builder.Property(p => p.DataAlteracao)
                .IsRequired();

            builder.OwnsOne(p => p.ValorTotal, preco =>
            {
                preco.Property(p => p.Valor)
                    .HasColumnName("ValorTotal")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();
            });

            builder.HasMany(p => p.Jogos)
                .WithOne()
                .HasForeignKey(pj => pj.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Pedido.Jogos))
                ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
