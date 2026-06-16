using Fcg.Catalogo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Catalogo.Infrastructure.Persistence.Mappings
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

            // Configuração do relacionamento de Biblioteca com Jogo
            builder.HasOne(b => b.Jogo)
                .WithMany()
                .HasForeignKey(b => b.JogoId)
                .OnDelete(DeleteBehavior.Restrict); // Usando Restrict para evitar delete em cascata indesejado de Jogos

            // Regra de Negócio: Garantir que um usuário tenha apenas um registro único de cada jogo em sua biblioteca
            builder.HasIndex(b => new { b.UsuarioId, b.JogoId })
                .IsUnique();

            // Índices adicionais para otimização de consultas por usuário ou por jogo separadamente
            builder.HasIndex(b => b.UsuarioId);
            builder.HasIndex(b => b.JogoId);
        }
    }
}
