using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.UpdateGame;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.Resources;
using FluentAssertions;
using System;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Catalog.Commands.Admin.UpdateGame
{
    public class UpdateGameCommandValidatorTests
    {
        private readonly UpdateGameValidator _validator = new();

        [Theory]
        [InlineData(null)]
        [InlineData("/covers/neon-rally.svg")]
        public void Validar_ComCapaNulaOuValida_NaoDeveTerErroNaCapa(string? capa)
        {
            // Act
            var resultado = _validator.Validate(new UpdateGameCommand { NewCoverUrl = capa });

            // Assert
            resultado.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateGameCommand.NewCoverUrl));
        }

        private static UpdateGameCommand CriarComandoValido(string nome, string descricao) => new()
        {
            GameId = Guid.NewGuid(),
            NewName = nome,
            NewDescription = descricao,
            NewPrice = 120.00m,
            NewGenre = GameGenre.Adventure
        };

        [Fact]
        public void Validar_ComNomeEDescricaoComEspacosENumeros_DeveSerValido()
        {
            // Arrange
            var command = CriarComandoValido("Zelda Breath of the Wild 2", "Mundo aberto com 120 santuários, puzzles e muita exploração.");

            // Act
            var resultado = _validator.Validate(command);

            // Assert
            resultado.IsValid.Should().BeTrue(string.Join("; ", resultado.Errors));
        }

        [Theory]
        [InlineData("Street Fighter: Edição 6")]
        [InlineData("Half-Life 3")]
        public void Validar_ComNomeComDoisPontosOuHifen_NaoDeveTerErroNoNome(string nome)
        {
            // Act
            var resultado = _validator.Validate(CriarComandoValido(nome, "Descrição válida do jogo."));

            // Assert
            resultado.Errors.Should().NotContain(e => e.PropertyName == nameof(UpdateGameCommand.NewName));
        }

        [Fact]
        public void Validar_ComNomeComCaractereForaDoPermitido_DeveApontarErroNoNome()
        {
            // Act
            var resultado = _validator.Validate(CriarComandoValido("Jogo <script>", "Descrição válida do jogo."));

            // Assert
            resultado.Errors.Should().Contain(e => e.PropertyName == nameof(UpdateGameCommand.NewName));
        }

        [Fact]
        public void Validar_ComCapaEmFormatoInvalido_DeveApontarErroNaCapa()
        {
            // Act
            var resultado = _validator.Validate(new UpdateGameCommand { NewCoverUrl = "ftp://host/capa.png" });

            // Assert
            resultado.Errors.Should().ContainSingle(e => e.PropertyName == nameof(UpdateGameCommand.NewCoverUrl))
                .Which.ErrorMessage.Should().Be(CatalogMessages.CoverUrlFormatInvalid);
        }
    }
}
