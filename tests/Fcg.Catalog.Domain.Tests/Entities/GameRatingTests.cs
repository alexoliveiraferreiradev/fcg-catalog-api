using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;

namespace Fcg.Catalog.Domain.Tests.Entities
{
    public class GameRatingTests
    {
        private static Game CriarJogo() =>
            new Game(new Name("Super Mario"), new Description("Game clássico de Platformer de encanadores"), new Price(100m), GameGenre.Platformer);

        [Fact]
        public void AverageRating_SemAvaliacoes_DeveSerNulo()
        {
            var game = CriarJogo();

            Assert.Null(game.AverageRating);
            Assert.Equal(0, game.RatingCount);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(5.5)]
        [InlineData(double.NaN)]
        public void Avaliation_ForaDaFaixa_DeveLancarDomainException(double valor)
        {
            Assert.Throws<DomainException>(() => new Avaliation(valor));
        }

        [Fact]
        public void GamesRating_EventoMaisAntigo_NaoDeveTrocarNota()
        {
            var agora = DateTime.UtcNow;
            var rating = new GamesRating(Guid.NewGuid(), Guid.NewGuid(), new Avaliation(4), agora);

            var mudou = rating.UpdateRating(new Avaliation(1), agora.AddMinutes(-1));

            Assert.False(mudou);
            Assert.Equal(4, rating.Rating.Value);
        }

        [Fact]
        public void GamesRating_EventoMaisNovo_DeveTrocarNota()
        {
            var agora = DateTime.UtcNow;
            var rating = new GamesRating(Guid.NewGuid(), Guid.NewGuid(), new Avaliation(4), agora);

            var mudou = rating.UpdateRating(new Avaliation(2), agora.AddMinutes(1));

            Assert.True(mudou);
            Assert.Equal(2, rating.Rating.Value);
        }

        [Fact]
        public void GamesRating_SemUsuario_DeveLancarDomainException()
        {
            Assert.Throws<DomainException>(() =>
                new GamesRating(Guid.NewGuid(), Guid.Empty, new Avaliation(4), DateTime.UtcNow));
        }
    }
}
