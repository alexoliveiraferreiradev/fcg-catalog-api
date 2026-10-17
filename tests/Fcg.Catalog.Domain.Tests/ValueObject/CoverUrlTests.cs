using Fcg.Catalog.Domain.Resources;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;

namespace Fcg.Catalog.Domain.Tests.ValueObject
{
    public class CoverUrlTests
    {
        [Theory]
        [InlineData("/covers/pixel-pals.svg")]
        [InlineData("http://localhost:5173/covers/neon-rally.svg")]
        [InlineData("https://cdn.exemplo.com/capas/elden-ring.jpg")]
        public void Construtor_ComCaminhoOuUrlHttp_DeveCriarCapa(string valor)
        {
            // Act
            var capa = new CoverUrl(valor);

            // Assert
            Assert.Equal(valor, capa.Value);
        }

        [Fact]
        public void Construtor_ComEspacosNasPontas_DeveRemoverEspacos()
        {
            // Act
            var capa = new CoverUrl("  /covers/pixel-pals.svg  ");

            // Assert
            Assert.Equal("/covers/pixel-pals.svg", capa.Value);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Construtor_ComValorVazio_DeveLancarDomainException(string? valor)
        {
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => new CoverUrl(valor!));
            Assert.Equal(CatalogMessages.CoverUrlRequired, exception.Message);
        }

        [Fact]
        public void Construtor_ComMaisDe500Caracteres_DeveLancarDomainException()
        {
            // Arrange
            var valor = "/covers/" + new string('a', CoverUrl.MaxLength);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => new CoverUrl(valor));
            Assert.Equal(CatalogMessages.CoverUrlLengthInvalid, exception.Message);
        }

        [Theory]
        [InlineData("//outro-host.com/capa.png")]
        [InlineData(@"/\outro-host.com/capa.png")]
        [InlineData("javascript:alert(1)")]
        [InlineData("ftp://host/capa.png")]
        [InlineData("covers/sem-barra.svg")]
        [InlineData("data:image/png;base64,AAAA")]
        public void Construtor_ComFormatoInvalido_DeveLancarDomainException(string valor)
        {
            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => new CoverUrl(valor));
            Assert.Equal(CatalogMessages.CoverUrlFormatInvalid, exception.Message);
        }

        [Fact]
        public void Equals_ComMesmoValor_DeveSerIgual()
        {
            // Assert
            Assert.Equal(new CoverUrl("/covers/a.svg"), new CoverUrl("/covers/a.svg"));
            Assert.NotEqual(new CoverUrl("/covers/a.svg"), new CoverUrl("/covers/b.svg"));
        }
    }
}
