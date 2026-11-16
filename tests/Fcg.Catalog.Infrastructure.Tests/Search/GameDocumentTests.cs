using Fcg.Catalog.Infrastructure.Search;
using FluentAssertions;

namespace Fcg.Catalog.Infrastructure.Tests.Search
{
    /// <summary>Preço atual calculado na leitura do índice, a partir da promoção guardada no documento.</summary>
    public class GameDocumentTests
    {
        private static readonly DateTime Now = new(2026, 11, 15, 12, 0, 0, DateTimeKind.Utc);

        [Fact]
        public void CurrentPriceAt_DeveSerPrecoOriginal_QuandoNaoHaPromocao()
        {
            var document = new GameDocument { OriginalPrice = 100m };

            document.CurrentPriceAt(Now).Should().Be(100m);
        }

        [Fact]
        public void CurrentPriceAt_DeveSerPrecoPromocional_QuandoPromocaoEstaEmVigor()
        {
            var document = new GameDocument
            {
                OriginalPrice = 100m,
                PromotionPrice = 60m,
                PromotionStartDate = Now.AddDays(-1),
                PromotionEndDate = Now.AddDays(1)
            };

            document.CurrentPriceAt(Now).Should().Be(60m);
        }

        [Fact]
        public void CurrentPriceAt_DeveSerPrecoOriginal_QuandoPromocaoAindaNaoComecou()
        {
            var document = new GameDocument
            {
                OriginalPrice = 100m,
                PromotionPrice = 60m,
                PromotionStartDate = Now.AddDays(1),
                PromotionEndDate = Now.AddDays(5)
            };

            document.CurrentPriceAt(Now).Should().Be(100m);
        }

        [Fact]
        public void CurrentPriceAt_DeveSerPrecoOriginal_QuandoPromocaoJaVenceu()
        {
            // O índice não é atualizado quando a promoção vence: o preço precisa voltar sozinho na leitura.
            var document = new GameDocument
            {
                OriginalPrice = 100m,
                PromotionPrice = 60m,
                PromotionStartDate = Now.AddDays(-5),
                PromotionEndDate = Now.AddDays(-1)
            };

            document.CurrentPriceAt(Now).Should().Be(100m);
        }

        [Fact]
        public void CurrentPriceAt_DeveSerPrecoPromocional_NoPrimeiroENoUltimoInstanteDaPromocao()
        {
            // Mesmo critério do SQL (GETUTCDATE() BETWEEN StartDate AND EndDate): os dois limites contam.
            var document = new GameDocument
            {
                OriginalPrice = 100m,
                PromotionPrice = 60m,
                PromotionStartDate = Now,
                PromotionEndDate = Now.AddDays(3)
            };

            document.CurrentPriceAt(Now).Should().Be(60m);
            document.CurrentPriceAt(Now.AddDays(3)).Should().Be(60m);
        }
    }
}
