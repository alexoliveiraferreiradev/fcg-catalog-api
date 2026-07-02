using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Tests.Entities
{
    public class PromocaoTests
    {
        private Preco ObterPrecoValido(decimal valor = 50m) => new Preco(valor);
        private Periodo ObterPeriodoValido(int diasFim = 5) => new Periodo(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(diasFim));

        #region Construtor Tests

        [Fact]
        public void Construtor_ComDadosValidos_DeveCriarPromocaoAtiva()
        {
            // Arrange
            var jogoId = Guid.NewGuid();
            var valor = ObterPrecoValido();
            var periodo = ObterPeriodoValido();

            // Act
            var promocao = new Promocao(jogoId, valor, periodo);

            // Assert
            Assert.Equal(jogoId, promocao.JogoId);
            Assert.Equal(valor, promocao.ValorPromocao);
            Assert.Equal(periodo, promocao.Periodo);
            Assert.True(promocao.Ativo);
            Assert.True((DateTime.UtcNow - promocao.DataCadastro).TotalSeconds < 5);
            Assert.Equal(promocao.DataCadastro, promocao.DataAlteracao);
        }

        [Fact]
        public void Construtor_ComJogoIdVazio_DeveLancarDomainException()
        {
            // Arrange
            var jogoIdVazio = Guid.Empty;
            var valor = ObterPrecoValido();
            var periodo = ObterPeriodoValido();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => new Promocao(jogoIdVazio, valor, periodo));
            Assert.Equal(MensagensDominio.JogoNaoEncontrado, excecao.Message);
        }

        [Fact]
        public void Construtor_ComPeriodoNulo_DeveLancarDomainException()
        {
            // Arrange
            var jogoId = Guid.NewGuid();
            var valor = ObterPrecoValido();
            Periodo periodoNulo = null!;

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => new Promocao(jogoId, valor, periodoNulo));
            Assert.Equal(MensagensDominio.PeriodoObrigatorio, excecao.Message);
        }

        #endregion

        #region EstaValida Tests

        [Fact]
        public void EstaValida_PromocaoAtivaEPeriodoValido_DeveRetornarTrue()
        {
            // Arrange
            var promocao = new Promocao(Guid.NewGuid(), ObterPrecoValido(), ObterPeriodoValido());

            // Act & Assert
            Assert.True(promocao.EstaValida());
        }

        [Fact]
        public void EstaValida_PromocaoInativa_DeveRetornarFalse()
        {
            // Arrange
            var promocao = new Promocao(Guid.NewGuid(), ObterPrecoValido(), ObterPeriodoValido());
            promocao.Desativar();

            // Act & Assert
            Assert.False(promocao.EstaValida());
        }

        [Fact]
        public void EstaValida_ForaDoPeriodo_DeveRetornarFalse()
        {
            // Arrange
            // Período no passado
            var dataInicio = DateTime.UtcNow.AddDays(-10);
            var dataFim = DateTime.UtcNow.AddDays(-2);
            var periodoExpirado = new Periodo(dataInicio, dataFim);

            var promocao = new Promocao(Guid.NewGuid(), ObterPrecoValido(), periodoExpirado);

            // Act & Assert
            Assert.False(promocao.EstaValida());
        }

        #endregion

        #region Desativar Tests

        [Fact]
        public void Desativar_PromocaoAtiva_DeveDesativar()
        {
            // Arrange
            var promocao = new Promocao(Guid.NewGuid(), ObterPrecoValido(), ObterPeriodoValido());

            // Act
            promocao.Desativar();

            // Assert
            Assert.False(promocao.Ativo);
            Assert.True((DateTime.UtcNow - promocao.DataAlteracao).TotalSeconds < 5);
        }

        [Fact]
        public void Desativar_PromocaoInativa_DeveLancarDomainException()
        {
            // Arrange
            var promocao = new Promocao(Guid.NewGuid(), ObterPrecoValido(), ObterPeriodoValido());
            promocao.Desativar();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => promocao.Desativar());
            Assert.Equal(MensagensDominio.PromocaoInativa, excecao.Message);
        }

        #endregion

        #region AtualizarPromocao Tests

        [Fact]
        public void AtualizarPromocao_ComNovosDados_DeveAtualizarValores()
        {
            // Arrange
            var promocao = new Promocao(Guid.NewGuid(), ObterPrecoValido(80m), ObterPeriodoValido());
            var novoPreco = ObterPrecoValido(60m);
            var novaDataFim = DateTime.UtcNow.AddDays(15);

            // Act
            promocao.AtualizarPromocao(novoPreco, novaDataFim);

            // Assert
            Assert.Equal(novoPreco, promocao.ValorPromocao);
            Assert.Equal(novaDataFim, promocao.Periodo.DataFim);
            Assert.True((DateTime.UtcNow - promocao.DataAlteracao).TotalSeconds < 5);
        }

        #endregion
    }
}
