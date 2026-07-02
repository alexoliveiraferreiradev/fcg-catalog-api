using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Tests.Entities
{
    public class JogoTests
    {
        private Name ObterNomeValido() => new Name("Super Mario");
        private Description ObterDescricaoValida() => new Description("Game clássico de plataforma de encanadores");
        private Price ObterPrecoValido(decimal Amount = 100m) => new Price(Amount);

        #region Construtor Tests

        [Fact]
        public void Construtor_ComDadosValidos_DeveCriarJogoAtivo()
        {
            // Arrange
            var Name = ObterNomeValido();
            var Description = ObterDescricaoValida();
            var Price = ObterPrecoValido();
            var Genre = GameGenre.Plataforma;

            // Act
            var Game = new Game(Name, Description, Price, Genre);

            // Assert
            Assert.Equal(Name, Game.Name);
            Assert.Equal(Description, Game.Description);
            Assert.Equal(Price, Game.BasePrice);
            Assert.Equal(Genre, Game.Genre);
            Assert.True(Game.IsActive);
            Assert.Empty(Game.Promotions);
            Assert.True((DateTime.UtcNow - Game.CreatedAt).TotalSeconds < 5);
            Assert.Equal(Game.CreatedAt, Game.UpdatedAt);
        }

        [Fact]
        public void Construtor_ComGeneroInvalido_DeveLancarDomainException()
        {
            // Arrange
            var Name = ObterNomeValido();
            var Description = ObterDescricaoValida();
            var Price = ObterPrecoValido();
            var generoInvalido = (GameGenre)0;

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => new Game(Name, Description, Price, generoInvalido));
            Assert.Equal(DomainMessages.GameGenreInvalid, excecao.Message);
        }

        #endregion

        #region Deactivate/Reactivate Tests

        [Fact]
        public void Desativar_JogoAtivo_DeveDesativar()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(), GameGenre.Acao);

            // Act
            Game.Deactivate();

            // Assert
            Assert.False(Game.IsActive);
            Assert.True((DateTime.UtcNow - Game.UpdatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void Desativar_JogoInativo_DeveLancarDomainException()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(), GameGenre.Acao);
            Game.Deactivate();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => Game.Deactivate());
            Assert.Equal(DomainMessages.GameIsDeactivated, excecao.Message);
        }

        [Fact]
        public void Reativar_JogoInativo_DeveReativar()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(), GameGenre.Acao);
            Game.Deactivate();

            // Act
            Game.Reactivate();

            // Assert
            Assert.True(Game.IsActive);
            Assert.True((DateTime.UtcNow - Game.UpdatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void Reativar_JogoAtivo_DeveLancarDomainException()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(), GameGenre.Acao);

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => Game.Reactivate());
            Assert.Equal(DomainMessages.GameAlreadyActive, excecao.Message);
        }

        #endregion

        #region Update Tests

        [Fact]
        public void Atualizar_ComDadosValidos_DeveAtualizarCampos()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GameGenre.Acao);
            var novoNome = new Name("Zelda Breath of the Wild");
            var novaDescricao = new Description("Game de RPG e mundo aberto com grande exploração");
            var novoPreco = ObterPrecoValido(150m);
            var novoGenero = GameGenre.RPG;

            // Act
            Game.Update(novoNome, novaDescricao, novoPreco, novoGenero);

            // Assert
            Assert.Equal(novoNome, Game.Name);
            Assert.Equal(novaDescricao, Game.Description);
            Assert.Equal(novoPreco, Game.BasePrice);
            Assert.Equal(novoGenero, Game.Genre);
        }

        [Fact]
        public void Atualizar_JogoInativo_DeveLancarDomainException()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(), GameGenre.Acao);
            Game.Deactivate();

            var novoNome = new Name("Zelda Breath of the Wild");
            var novaDescricao = new Description("Game de RPG e mundo aberto com grande exploração");
            var novoPreco = ObterPrecoValido(150m);
            var novoGenero = GameGenre.RPG;

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => Game.Update(novoNome, novaDescricao, novoPreco, novoGenero));
            Assert.Equal(DomainMessages.GameIsDeactivated, excecao.Message);
        }

        #endregion

        #region Promotion Tests

        [Fact]
        public void AdicionarPromocao_ComPrecoMenorQuePrecoBase_DeveAdicionar()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GameGenre.Acao);
            var precoPromocional = ObterPrecoValido(80m);
            var EndDate = new Period(DateTime.UtcNow.AddDays(7));

            // Act
            Game.AddPromotion(precoPromocional, EndDate);

            // Assert
            Assert.Single(Game.Promotions);
            var promo = Game.Promotions.First();
            Assert.Equal(precoPromocional, promo.ValorPromocao);
            Assert.Equal(EndDate, promo.Period);
            Assert.True(promo.IsActive);
        }

        [Fact]
        public void AdicionarPromocao_ComPrecoMaiorOuIgualAoPrecoBase_DeveLancarDomainException()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GameGenre.Acao);
            var precoPromocionalInvalido = ObterPrecoValido(100m); // igual
            var EndDate = new Period(DateTime.UtcNow.AddDays(7));

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => Game.AddPromotion(precoPromocionalInvalido, EndDate));
            Assert.Equal(DomainMessages.PromotionValueHigher, excecao.Message);
        }

        [Fact]
        public void AlteraPromocao_ComDadosValidos_DeveAtualizarPromocao()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GameGenre.Acao);
            var precoPromocional = ObterPrecoValido(80m);
            var EndDate = new Period(DateTime.UtcNow.AddDays(7));
            Game.AddPromotion(precoPromocional, EndDate);
            var promoId = Game.Promotions.First().Id;

            var novoPreco = ObterPrecoValido(60m);
            var novaDataFim = DateTime.UtcNow.AddDays(10);

            // Act
            Game.UpdatePromotion(promoId, novoPreco, novaDataFim);

            // Assert
            var promo = Game.Promotions.First();
            Assert.Equal(novoPreco, promo.ValorPromocao);
            Assert.Equal(novaDataFim, promo.Period.EndDate);
        }

        [Fact]
        public void AlteraPromocao_ComPrecoMaiorOuIgualAoPrecoBase_DeveLancarDomainException()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GameGenre.Acao);
            var precoPromocional = ObterPrecoValido(80m);
            var EndDate = new Period(DateTime.UtcNow.AddDays(7));
            Game.AddPromotion(precoPromocional, EndDate);
            var promoId = Game.Promotions.First().Id;

            var novoPrecoInvalido = ObterPrecoValido(110m); // Maior que preço base
            var novaDataFim = DateTime.UtcNow.AddDays(10);

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => Game.UpdatePromotion(promoId, novoPrecoInvalido, novaDataFim));
            Assert.Equal(DomainMessages.PromotionValueHigher, excecao.Message);
        }

        [Fact]
        public void ObterPrecoAtual_ComPromocaoAtivaEValida_DeveRetornarPrecoDaPromocao()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GameGenre.Acao);
            var precoPromocional = ObterPrecoValido(75m);
            // Período contendo a data atual
            var StartDate = DateTime.UtcNow.AddDays(-1);
            var EndDate = DateTime.UtcNow.AddDays(5);
            var Period = new Period(StartDate, EndDate);
            Game.AddPromotion(precoPromocional, Period);

            // Act
            var precoAtual = Game.GetCurrentPrice();

            // Assert
            Assert.Equal(precoPromocional.Amount, precoAtual.Amount);
        }

        [Fact]
        public void ObterPrecoAtual_SemPromocaoValida_DeveRetornarPrecoBase()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GameGenre.Acao);

            // Act
            var precoAtual = Game.GetCurrentPrice();

            // Assert
            Assert.Equal(Game.BasePrice.Amount, precoAtual.Amount);
        }

        [Fact]
        public void DeactivatePromotion_PromocaoExistente_DeveDesativar()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GameGenre.Acao);
            Game.AddPromotion(ObterPrecoValido(80m), new Period(DateTime.UtcNow.AddDays(7)));
            var promoId = Game.Promotions.First().Id;

            // Act
            Game.DeactivatePromotion(promoId);

            // Assert
            Assert.False(Game.Promotions.First().IsActive);
        }

        [Fact]
        public void DeactivatePromotion_PromocaoNaoEncontrada_DeveLancarDomainException()
        {
            // Arrange
            var Game = new Game(ObterNomeValido(), ObterDescricaoValida(), ObterPrecoValido(100m), GameGenre.Acao);
            var promoIdInexistente = Guid.NewGuid();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => Game.DeactivatePromotion(promoIdInexistente));
            Assert.Equal(DomainMessages.PromotionNotFound, excecao.Message);
        }

        #endregion
    }
}
