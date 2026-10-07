using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Tests.Entities
{
    public class PromotionTests
    {
        private Price ObterPrecoValido(decimal Amount = 50m) => new Price(Amount);
        private Period ObterPeriodoValido(int diasFim = 5) => new Period(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(diasFim));

        #region Construtor Tests

        [Fact]
        public void Construtor_ComDadosValidos_DeveCriarPromocaoAtiva()
        {
            // Arrange
            var GameId = Guid.NewGuid();
            var Amount = ObterPrecoValido();
            var Period = ObterPeriodoValido();

            // Act
            var Promotion = new Promotion(GameId, Amount, Period);

            // Assert
            Assert.Equal(GameId, Promotion.GameId);
            Assert.Equal(Amount, Promotion.ValorPromocao);
            Assert.Equal(Period, Promotion.Period);
            Assert.True(Promotion.IsActive);
            Assert.True((DateTime.UtcNow - Promotion.CreatedAt).TotalSeconds < 5);
            Assert.Equal(Promotion.CreatedAt, Promotion.UpdatedAt);
        }

        [Fact]
        public void Construtor_ComJogoIdVazio_DeveLancarDomainException()
        {
            // Arrange
            var jogoIdVazio = Guid.Empty;
            var Amount = ObterPrecoValido();
            var Period = ObterPeriodoValido();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => new Promotion(jogoIdVazio, Amount, Period));
            Assert.Equal(DomainMessages.GameNotFound, excecao.Message);
        }

        [Fact]
        public void Construtor_ComPeriodoNulo_DeveLancarDomainException()
        {
            // Arrange
            var GameId = Guid.NewGuid();
            var Amount = ObterPrecoValido();
            Period periodoNulo = null!;

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => new Promotion(GameId, Amount, periodoNulo));
            Assert.Equal(DomainMessages.PeriodRequired, excecao.Message);
        }

        #endregion

        #region IsValid Tests

        [Fact]
        public void EstaValida_PromocaoAtivaEPeriodoValido_DeveRetornarTrue()
        {
            // Arrange
            var Promotion = new Promotion(Guid.NewGuid(), ObterPrecoValido(), ObterPeriodoValido());

            // Act & Assert
            Assert.True(Promotion.IsValid());
        }

        [Fact]
        public void EstaValida_PromocaoInativa_DeveRetornarFalse()
        {
            // Arrange
            var Promotion = new Promotion(Guid.NewGuid(), ObterPrecoValido(), ObterPeriodoValido());
            Promotion.Deactivate();

            // Act & Assert
            Assert.False(Promotion.IsValid());
        }

        [Fact]
        public void EstaValida_ForaDoPeriodo_DeveRetornarFalse()
        {
            // Arrange
            // Período no passado
            var StartDate = DateTime.UtcNow.AddDays(-10);
            var EndDate = DateTime.UtcNow.AddDays(-2);
            var periodoExpirado = new Period(StartDate, EndDate);

            var Promotion = new Promotion(Guid.NewGuid(), ObterPrecoValido(), periodoExpirado);

            // Act & Assert
            Assert.False(Promotion.IsValid());
        }

        #endregion

        #region Deactivate Tests

        [Fact]
        public void Desativar_PromocaoAtiva_DeveDesativar()
        {
            // Arrange
            var Promotion = new Promotion(Guid.NewGuid(), ObterPrecoValido(), ObterPeriodoValido());

            // Act
            Promotion.Deactivate();

            // Assert
            Assert.False(Promotion.IsActive);
            Assert.True((DateTime.UtcNow - Promotion.UpdatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void Desativar_PromocaoInativa_DeveLancarDomainException()
        {
            // Arrange
            var Promotion = new Promotion(Guid.NewGuid(), ObterPrecoValido(), ObterPeriodoValido());
            Promotion.Deactivate();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => Promotion.Deactivate());
            Assert.Equal(DomainMessages.PromotionAlreadyDeactivated, excecao.Message);
        }

        #endregion

        #region UpdatePromotion Tests

        [Fact]
        public void UpdatePromotion_ComNovosDados_DeveAtualizarValores()
        {
            // Arrange
            var Promotion = new Promotion(Guid.NewGuid(), ObterPrecoValido(80m), ObterPeriodoValido());
            var novoPreco = ObterPrecoValido(60m);
            var novaDataFim = DateTime.UtcNow.AddDays(15);

            // Act
            Promotion.UpdatePromotion(novoPreco, novaDataFim);

            // Assert
            Assert.Equal(novoPreco, Promotion.ValorPromocao);
            Assert.Equal(novaDataFim, Promotion.Period.EndDate);
            Assert.True((DateTime.UtcNow - Promotion.UpdatedAt).TotalSeconds < 5);
        }

        #endregion
    }
}
