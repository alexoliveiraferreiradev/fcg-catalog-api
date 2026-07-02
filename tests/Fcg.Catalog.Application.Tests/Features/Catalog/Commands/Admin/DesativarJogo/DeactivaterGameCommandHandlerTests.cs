using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DesativarJogo;
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

namespace Fcg.Catalog.Application.Tests.Features.Catalog.Commands.Admin.DesativarJogo
{
    public class DesativarJogoCommandHandlerTests
    {
        private readonly Mock<IGameRepository> _jogoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<DesativarJogoCommandHandler>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DesativarJogoCommandHandler _handler;

        public DesativarJogoCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IGameRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DesativarJogoCommandHandler>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new DesativarJogoCommandHandler(
                _jogoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mediatorMock.Object
            );
        }

        private Game CriarJogoValido()
        {
            return new Game(
                new Name("Game Teste"),
                new Description("Description do Game longa"),
                new Price(100.00m),
                GameGenre.RPG
            );
        }

        [Fact]
        public async Task Handle_DeveDesativarJogoEPublicarEvento_QuandoJogoExistirEEstiverAtivo()
        {
            // Arrange
            var Game = CriarJogoValido();
            var command = new DesativarJogoCommand(Game.Id);

            _jogoRepositoryMock.Setup(r => r.GetById(command.GameId)).ReturnsAsync(Game);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Game.IsActive.Should().BeFalse();
            _jogoRepositoryMock.Verify(r => r.Update(Game), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<GameDeactivatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogoNaoForEncontrado()
        {
            // Arrange
            var command = new DesativarJogoCommand(Guid.NewGuid());
            _jogoRepositoryMock.Setup(r => r.GetById(command.GameId)).ReturnsAsync((Game)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
