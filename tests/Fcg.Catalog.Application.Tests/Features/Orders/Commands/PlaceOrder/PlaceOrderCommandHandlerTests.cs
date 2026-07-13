using Fcg.Catalog.Application.Features.Orders.Commands.PlaceOrder;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.SharedContracts.MessageContracts;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace Fcg.Catalog.Application.Tests.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandHandlerTests
    {
        private readonly Mock<IGameRepository> _gameRepositoryMock;
        private readonly Mock<ILibraryRepository> _libraryRepositoryMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ILogger<PlaceOrderCommandHandler>> _loggerMock;
        private readonly PlaceOrderCommandHandler _handler;

        public PlaceOrderCommandHandlerTests()
        {
            _gameRepositoryMock = new Mock<IGameRepository>();
            _libraryRepositoryMock = new Mock<ILibraryRepository>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _loggerMock = new Mock<ILogger<PlaceOrderCommandHandler>>();

            _handler = new PlaceOrderCommandHandler(
                _publishEndpointMock.Object,
                _libraryRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _orderRepositoryMock.Object,
                _gameRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        private Game CriarJogoValido()
        {
            return new Game(
                new Name("Game Teste"),
                new Description("Description do Game Teste longo para passar na validacao"),
                new Price(100.00m),
                GameGenre.RPG
            );
        }

        [Fact]
        public async Task Handle_DevePublicarEventoERetornarTrue_QuandoPedidoForValido()
        {
            // Arrange
            var UserId = Guid.NewGuid();
            var game = CriarJogoValido();
            var jogoIdReal = game.Id;

            var command = new PlaceOrderCommand(UserId, "User", "user@teste.com", new List<Guid> { jogoIdReal });

            _gameRepositoryMock
                .Setup(r => r.GetGamesByIds(command.JogosIds))
                .ReturnsAsync(new List<Game> { game });

            _libraryRepositoryMock
                .Setup(r => r.GetPurchasedGamesByUser(UserId))
                .ReturnsAsync(new List<Guid>()); // UserLibrary vazia

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            
            _publishEndpointMock.Verify(p => p.Publish(It.Is<OrderPlacedEvent>(e => 
                e.UserId == UserId && 
                e.AmountPrice == 100.00m && 
                e.GameIds.Contains(jogoIdReal)), It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogosInexistentes()
        {
            // Arrange
            var UserId = Guid.NewGuid();
            var jogoInexistenteId = Guid.NewGuid();
            
            var command = new PlaceOrderCommand(UserId, "User", "user@teste.com", new List<Guid> { jogoInexistenteId });

            _gameRepositoryMock
                .Setup(r => r.GetGamesByIds(command.JogosIds))
                .ReturnsAsync(new List<Game>()); // Nenhum Game retornado

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("*A compra foi cancelada. Os seguintes IDs de Games não foram encontrados no catálogo*");
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoUsuarioJaPossuirOJogo()
        {
            // Arrange
            var UserId = Guid.NewGuid();
            var game = CriarJogoValido();
            var GameId = game.Id;
            
            var command = new PlaceOrderCommand(UserId, "User", "user@teste.com", new List<Guid> { GameId });

            _gameRepositoryMock
                .Setup(r => r.GetGamesByIds(command.JogosIds))
                .ReturnsAsync(new List<Game> { game });

            _libraryRepositoryMock
                .Setup(r => r.GetPurchasedGamesByUser(UserId))
                .ReturnsAsync(new List<Guid> { GameId }); // Usuário já possui este Game

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("*já possui o Game*");
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogoEstiverInativo()
        {
            // Arrange
            var UserId = Guid.NewGuid();
            var game = CriarJogoValido();
            game.Deactivate();
            var GameId = game.Id;
            
            var command = new PlaceOrderCommand(UserId, "User", "user@teste.com", new List<Guid> { GameId });

            _gameRepositoryMock
                .Setup(r => r.GetGamesByIds(command.JogosIds))
                .ReturnsAsync(new List<Game> { game });

            _libraryRepositoryMock
                .Setup(r => r.GetPurchasedGamesByUser(UserId))
                .ReturnsAsync(new List<Guid>()); 

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("*não está mais disponível para aquisição.*");
        }
    }
}
