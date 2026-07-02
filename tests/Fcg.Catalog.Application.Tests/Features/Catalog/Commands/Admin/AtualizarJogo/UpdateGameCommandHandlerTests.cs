using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.UpdateGame;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Catalog.Commands.Admin.UpdateGame
{
    public class UpdateGameCommandHandlerTests
    {
        private readonly Mock<IGameRepository> _jogoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<UpdateGameCommandHandler>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly UpdateGameCommandHandler _handler;

        public UpdateGameCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IGameRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdateGameCommandHandler>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new UpdateGameCommandHandler(
                _jogoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mediatorMock.Object
            );
        }

        private Game CriarJogoValido()
        {
            return new Game(
                new Name("Game Antigo"),
                new Description("Description antiga longa"),
                new Price(100.00m),
                GameGenre.RPG
            );
        }

        [Fact]
        public async Task Handle_DeveUpdateGameEPublicarEvento_QuandoValido()
        {
            // Arrange
            var Game = CriarJogoValido();
            var command = new UpdateGameCommand
            {
                GameId = Game.Id,
                NovoNome = "Game Novo",
                NovaDescricao = "Nova Description longa",
                NovoPreco = 120.00m,
                NovoGenero = GameGenre.Acao
            };

            _jogoRepositoryMock.Setup(r => r.GetById(command.GameId)).ReturnsAsync(Game);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            Game.Name.Value.Should().Be(command.NovoNome);
            Game.BasePrice.Amount.Should().Be(command.NovoPreco);

            _jogoRepositoryMock.Verify(r => r.Update(Game), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<GameUpdatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogoNaoEncontrado()
        {
            // Arrange
            var command = new UpdateGameCommand { GameId = Guid.NewGuid() };
            _jogoRepositoryMock.Setup(r => r.GetById(command.GameId)).ReturnsAsync((Game)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
