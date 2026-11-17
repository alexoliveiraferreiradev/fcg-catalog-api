using Fcg.Catalog.Application.Features.Catalog.Commands.Players.RatingGame;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fcg.Catalog.Application.Tests.Features.Catalog.Commands.Players.RatingGame
{
    public class RatingGameCommandHandlerTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock = new();
        private readonly Mock<IGamesRatingRepository> _gamesRatingRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IMediator> _mediatorMock = new();
        private readonly Mock<ILogger<RatingGameCommandHandler>> _loggerMock = new();
        private readonly RatingGameCommandHandler _handler;

        public RatingGameCommandHandlerTests()
        {
            _handler = new RatingGameCommandHandler(
                _gameRepositoryMock.Object,
                _gamesRatingRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _mediatorMock.Object,
                _loggerMock.Object);
        }

        private Game CriarJogo()
        {
            var game = new Game(new Name("Super Mario"), new Description("Game clássico de Platformer"), new Price(100m), GameGenre.Plataforma);
            _gameRepositoryMock.Setup(r => r.GetById(game.Id)).ReturnsAsync(game);
            return game;
        }

        [Fact]
        public async Task Handle_PrimeiraAvaliacaoDoUsuario_DeveAdicionarNotaESomarNaMedia()
        {
            var game = CriarJogo();
            var userId = Guid.NewGuid();

            await _handler.Handle(new RatingGameCommand(game.Id, userId, 4, DateTime.UtcNow), CancellationToken.None);

            _gamesRatingRepositoryMock.Verify(r => r.Add(It.Is<GamesRating>(x => x.UserId == userId && x.Rating.Value == 4)), Times.Once);
            _gameRepositoryMock.Verify(r => r.ApplyRatingAsync(game.Id, 4, 1, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.Is<GameUpdatedEvent>(e => e.GameId == game.Id), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UsuarioReavalia_DeveTrocarNotaSemAumentarQuantidade()
        {
            var game = CriarJogo();
            var userId = Guid.NewGuid();
            var ratedAt = DateTime.UtcNow.AddMinutes(-5);
            var existing = new GamesRating(game.Id, userId, new Avaliation(2), ratedAt);
            _gamesRatingRepositoryMock.Setup(r => r.GetByGameAndUser(game.Id, userId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

            await _handler.Handle(new RatingGameCommand(game.Id, userId, 5, DateTime.UtcNow), CancellationToken.None);

            existing.Rating.Value.Should().Be(5);
            _gameRepositoryMock.Verify(r => r.ApplyRatingAsync(game.Id, 3, 0, It.IsAny<CancellationToken>()), Times.Once);
            _gamesRatingRepositoryMock.Verify(r => r.Add(It.IsAny<GamesRating>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_EventoAntigoDeReavaliacao_DeveIgnorar()
        {
            var game = CriarJogo();
            var userId = Guid.NewGuid();
            var ratedAt = DateTime.UtcNow;
            var existing = new GamesRating(game.Id, userId, new Avaliation(4), ratedAt);
            _gamesRatingRepositoryMock.Setup(r => r.GetByGameAndUser(game.Id, userId, It.IsAny<CancellationToken>())).ReturnsAsync(existing);

            await _handler.Handle(new RatingGameCommand(game.Id, userId, 1, ratedAt.AddMinutes(-1)), CancellationToken.None);

            existing.Rating.Value.Should().Be(4);
            _gameRepositoryMock.Verify(r => r.ApplyRatingAsync(It.IsAny<Guid>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }

        [Fact]
        public async Task Handle_JogoNaoEncontrado_DeveIgnorarSemLancarExcecao()
        {
            _gameRepositoryMock.Setup(r => r.GetById(It.IsAny<Guid>())).ReturnsAsync((Game?)null);

            var act = () => _handler.Handle(new RatingGameCommand(Guid.NewGuid(), Guid.NewGuid(), 4, DateTime.UtcNow), CancellationToken.None);

            await act.Should().NotThrowAsync();
            _gameRepositoryMock.Verify(r => r.ApplyRatingAsync(It.IsAny<Guid>(), It.IsAny<double>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
