using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddGame;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Catalog.Commands.Admin.AddGame
{
    public class AddGameCommandHandlerTests
    {
        private readonly Mock<IGameRepository> _jogoRepositoryMock;
        private readonly Mock<ILogger<AddGameCommandHandler>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AddGameCommandHandler _handler;

        public AddGameCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IGameRepository>();
            _loggerMock = new Mock<ILogger<AddGameCommandHandler>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new AddGameCommandHandler(
                _jogoRepositoryMock.Object,
                _loggerMock.Object,
                _mediatorMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeveAddGameEPublicarEvento_QuandoNomeEstiverDisponivel()
        {
            // Arrange
            var command = new AddGameCommand
            {
                Name = "Novo Game",
                Description = "Uma descrição super legal para o Game",
                Price = 150.0m,
                Genre = GameGenre.Estrategia
            };

            _jogoRepositoryMock
                .Setup(r => r.GameExistsWithName(command.Name))
                .ReturnsAsync(false);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Name.Should().Be(command.Name);
            response.OriginalPrice.Should().Be(command.Price);

            _jogoRepositoryMock.Verify(r => r.Add(It.Is<Game>(j => j.Name.Value == command.Name)), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<GameAddedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoNomeDoJogoJaExistir()
        {
            // Arrange
            var command = new AddGameCommand
            {
                Name = "Game Duplicado",
                Description = "Descrição",
                Price = 50.0m,
                Genre = GameGenre.Acao
            };

            _jogoRepositoryMock
                .Setup(r => r.GameExistsWithName(command.Name))
                .ReturnsAsync(true); // Name já existe

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("*Já existe um jogo com esse nome.*");
        }
    }
}
