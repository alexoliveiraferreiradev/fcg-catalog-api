using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Tests.Entities
{
    public class JogoTests
    {
        private Nome ObterNomeValido() => new Nome("Super Mario");
        private Descricao ObterDescricaoValida() => new Descricao("Jogo clássico de plataforma de encanadores");
        private Preco ObterPrecoValido(decimal valor = 100m) => new Preco(valor);

        #region Construtor Tests

        [Fact]
        public void Construtor_ComDadosValidos_DeveCriarJogoAtivo()
        {
            // Arrange
            var nome = ObterNomeValido();
            var descricao = ObterDescricaoValida();
            var preco = ObterPrecoValido();
            var genero = GeneroJogo.Plataforma;

            // Act
            var jogo = new Jogo(nome, descricao, preco, genero);

            // Assert
            Assert.Equal(nome, jogo.Nome);
            Assert.Equal(descricao, jogo.Descricao);
            Assert.Equal(preco, jogo.PrecoBase);
            Assert.Equal(genero, jogo.Genero);
            Assert.True(jogo.Ativo);
            Assert.Empty(jogo.Promocoes);
            Assert.True((DateTime.UtcNow - jogo.DataCadastro).TotalSeconds < 5);
            Assert.Equal(jogo.DataCadastro, jogo.DataAlteracao);
        }

        [Fact]
        public void Construtor_ComGeneroInvalido_DeveLancarDomainException()
        {
            // Arrange
            var nome = ObterNomeValido();
            var descricao = ObterDescricaoValida();
            var preco = ObterPrecoValido();
            var generoInvalido = (GeneroJogo)0;

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => new Jogo(nome, descricao, preco, generoInvalido));
            Assert.Equal(MensagensDominio.JogoGeneroObrigatorio, excecao.Message);
        }

        #endregion

        #region Desativar/Reativar Tests

        [Fact]
        public void Desativar_JogoAtivo_DeveDesativar()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(), GeneroJogo.Acao);

            // Act
            jogo.Desativar();

            // Assert
            Assert.False(jogo.Ativo);
            Assert.True((DateTime.UtcNow - jogo.DataAlteracao).TotalSeconds < 5);
        }

        [Fact]
        public void Desativar_JogoInativo_DeveLancarDomainException()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(), GeneroJogo.Acao);
            jogo.Desativar();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => jogo.Desativar());
            Assert.Equal(MensagensDominio.JogoInvalido, excecao.Message);
        }

        [Fact]
        public void Reativar_JogoInativo_DeveReativar()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(), GeneroJogo.Acao);
            jogo.Desativar();

            // Act
            jogo.Reativar();

            // Assert
            Assert.True(jogo.Ativo);
            Assert.True((DateTime.UtcNow - jogo.DataAlteracao).TotalSeconds < 5);
        }

        [Fact]
        public void Reativar_JogoAtivo_DeveLancarDomainException()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(), GeneroJogo.Acao);

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => jogo.Reativar());
            Assert.Equal(MensagensDominio.JogoAtivo, excecao.Message);
        }

        #endregion

        #region Atualizar Tests

        [Fact]
        public void Atualizar_ComDadosValidos_DeveAtualizarCampos()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GeneroJogo.Acao);
            var novoNome = new Nome("Zelda Breath of the Wild");
            var novaDescricao = new Descricao("Jogo de RPG e mundo aberto com grande exploração");
            var novoPreco = ObterPrecoValido(150m);
            var novoGenero = GeneroJogo.RPG;

            // Act
            jogo.Atualizar(novoNome, novaDescricao, novoPreco, novoGenero);

            // Assert
            Assert.Equal(novoNome, jogo.Nome);
            Assert.Equal(novaDescricao, jogo.Descricao);
            Assert.Equal(novoPreco, jogo.PrecoBase);
            Assert.Equal(novoGenero, jogo.Genero);
        }

        [Fact]
        public void Atualizar_JogoInativo_DeveLancarDomainException()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(), GeneroJogo.Acao);
            jogo.Desativar();

            var novoNome = new Nome("Zelda Breath of the Wild");
            var novaDescricao = new Descricao("Jogo de RPG e mundo aberto com grande exploração");
            var novoPreco = ObterPrecoValido(150m);
            var novoGenero = GeneroJogo.RPG;

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => jogo.Atualizar(novoNome, novaDescricao, novoPreco, novoGenero));
            Assert.Equal(MensagensDominio.JogoInvalido, excecao.Message);
        }

        #endregion

        #region Promocao Tests

        [Fact]
        public void AdicionarPromocao_ComPrecoMenorQuePrecoBase_DeveAdicionar()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GeneroJogo.Acao);
            var precoPromocional = ObterPrecoValido(80m);
            var dataFim = new Periodo(DateTime.UtcNow.AddDays(7));

            // Act
            jogo.AdicionarPromocao(precoPromocional, dataFim);

            // Assert
            Assert.Single(jogo.Promocoes);
            var promo = jogo.Promocoes.First();
            Assert.Equal(precoPromocional, promo.ValorPromocao);
            Assert.Equal(dataFim, promo.Periodo);
            Assert.True(promo.Ativo);
        }

        [Fact]
        public void AdicionarPromocao_ComPrecoMaiorOuIgualAoPrecoBase_DeveLancarDomainException()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GeneroJogo.Acao);
            var precoPromocionalInvalido = ObterPrecoValido(100m); // igual
            var dataFim = new Periodo(DateTime.UtcNow.AddDays(7));

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => jogo.AdicionarPromocao(precoPromocionalInvalido, dataFim));
            Assert.Equal(MensagensDominio.PromocaoValorMaior, excecao.Message);
        }

        [Fact]
        public void AlteraPromocao_ComDadosValidos_DeveAtualizarPromocao()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GeneroJogo.Acao);
            var precoPromocional = ObterPrecoValido(80m);
            var dataFim = new Periodo(DateTime.UtcNow.AddDays(7));
            jogo.AdicionarPromocao(precoPromocional, dataFim);
            var promoId = jogo.Promocoes.First().Id;

            var novoPreco = ObterPrecoValido(60m);
            var novaDataFim = DateTime.UtcNow.AddDays(10);

            // Act
            jogo.AlteraPromocao(promoId, novoPreco, novaDataFim);

            // Assert
            var promo = jogo.Promocoes.First();
            Assert.Equal(novoPreco, promo.ValorPromocao);
            Assert.Equal(novaDataFim, promo.Periodo.DataFim);
        }

        [Fact]
        public void AlteraPromocao_ComPrecoMaiorOuIgualAoPrecoBase_DeveLancarDomainException()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GeneroJogo.Acao);
            var precoPromocional = ObterPrecoValido(80m);
            var dataFim = new Periodo(DateTime.UtcNow.AddDays(7));
            jogo.AdicionarPromocao(precoPromocional, dataFim);
            var promoId = jogo.Promocoes.First().Id;

            var novoPrecoInvalido = ObterPrecoValido(110m); // Maior que preço base
            var novaDataFim = DateTime.UtcNow.AddDays(10);

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => jogo.AlteraPromocao(promoId, novoPrecoInvalido, novaDataFim));
            Assert.Equal(MensagensDominio.PromocaoValorMaior, excecao.Message);
        }

        [Fact]
        public void ObterPrecoAtual_ComPromocaoAtivaEValida_DeveRetornarPrecoDaPromocao()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GeneroJogo.Acao);
            var precoPromocional = ObterPrecoValido(75m);
            // Período contendo a data atual
            var dataInicio = DateTime.UtcNow.AddDays(-1);
            var dataFim = DateTime.UtcNow.AddDays(5);
            var periodo = new Periodo(dataInicio, dataFim);
            jogo.AdicionarPromocao(precoPromocional, periodo);

            // Act
            var precoAtual = jogo.ObterPrecoAtual();

            // Assert
            Assert.Equal(precoPromocional.Valor, precoAtual.Valor);
        }

        [Fact]
        public void ObterPrecoAtual_SemPromocaoValida_DeveRetornarPrecoBase()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GeneroJogo.Acao);

            // Act
            var precoAtual = jogo.ObterPrecoAtual();

            // Assert
            Assert.Equal(jogo.PrecoBase.Valor, precoAtual.Valor);
        }

        [Fact]
        public void DesativarPromocao_PromocaoExistente_DeveDesativar()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GeneroJogo.Acao);
            jogo.AdicionarPromocao(ObterPrecoValido(80m), new Periodo(DateTime.UtcNow.AddDays(7)));
            var promoId = jogo.Promocoes.First().Id;

            // Act
            jogo.DesativarPromocao(promoId);

            // Assert
            Assert.False(jogo.Promocoes.First().Ativo);
        }

        [Fact]
        public void DesativarPromocao_PromocaoNaoEncontrada_DeveLancarDomainException()
        {
            // Arrange
            var jogo = new Jogo(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GeneroJogo.Acao);
            var promoIdInexistente = Guid.NewGuid();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => jogo.DesativarPromocao(promoIdInexistente));
            Assert.Equal(MensagensDominio.PromocaoNaoEncontrada, excecao.Message);
        }

        #endregion
    }
}
