using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddGame;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.Resources;
using FluentAssertions;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Catalog.Commands.Admin.AddGame
{
    public class AddGameCommandValidatorTests
    {
        private readonly AddGameCommandValidator _validator = new();

        private static AddGameCommand CriarComando(string? capa) => new()
        {
            Name = "Pixel Pals",
            Description = "Plataforma em dupla para jogar no sofa",
            Price = 19.90m,
            Genre = GameGenre.Platformer,
            CoverUrl = capa
        };

        [Theory]
        [InlineData(null)]
        [InlineData("/covers/pixel-pals.svg")]
        [InlineData("https://cdn.exemplo.com/pixel-pals.png")]
        public void Validar_ComCapaNulaOuValida_NaoDeveTerErroNaCapa(string? capa)
        {
            // Act
            var resultado = _validator.Validate(CriarComando(capa));

            // Assert
            resultado.Errors.Should().NotContain(e => e.PropertyName == nameof(AddGameCommand.CoverUrl));
        }

        [Theory]
        [InlineData("", CatalogMessages.CoverUrlRequired)]
        [InlineData("javascript:alert(1)", CatalogMessages.CoverUrlFormatInvalid)]
        [InlineData("//outro-host.com/capa.png", CatalogMessages.CoverUrlFormatInvalid)]
        public void Validar_ComCapaInvalida_DeveApontarErroNaCapa(string capa, string mensagem)
        {
            // Act
            var resultado = _validator.Validate(CriarComando(capa));

            // Assert
            resultado.Errors.Should().ContainSingle(e => e.PropertyName == nameof(AddGameCommand.CoverUrl))
                .Which.ErrorMessage.Should().Be(mensagem);
        }

        [Fact]
        public void Validar_ComCapaMaiorQue500Caracteres_DeveApontarErroDeTamanho()
        {
            // Act
            var resultado = _validator.Validate(CriarComando("/covers/" + new string('a', 500)));

            // Assert
            resultado.Errors.Should().ContainSingle(e => e.PropertyName == nameof(AddGameCommand.CoverUrl))
                .Which.ErrorMessage.Should().Be(CatalogMessages.CoverUrlLengthInvalid);
        }
    }
}
