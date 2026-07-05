using Bogus;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Orders.Commands.PlaceOrder;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandHandlerTests
    {
        private readonly Mock<IGameQueryRepository> _gameQueryRepositoryMock;
        private readonly Mock<ILibraryQueryRepository> _libraryQueryRepositoryMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<ILogger<PlaceOrderCommandHandler>> _loggerMock;
        private readonly PlaceOrderCommandHandler _handler;

        public PlaceOrderCommandHandlerTests()
        {
            _gameQueryRepositoryMock = new Mock<IGameQueryRepository>();
            _libraryQueryRepositoryMock = new Mock<ILibraryQueryRepository>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _loggerMock = new Mock<ILogger<PlaceOrderCommandHandler>>();

            _handler = new PlaceOrderCommandHandler(
                _publishEndpointMock.Object,
                _libraryQueryRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _orderRepositoryMock.Object,
                _gameQueryRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        private GameResponse CriarJogoValido(Guid id, bool isActive = true)
        {
            return new GameResponse
            {
                Id = id,
                Name = "Game Teste",
                Description = "Description do Game Teste longo para passar na validacao",
                OriginalPrice = 100.00m,
                CurrentPrice = 100.00m,
                Genre = GameGenre.RPG,
                IsActive = isActive
            };
        }

        [Fact]
        public async Task Handle_DevePublicarEventoERetornarTrue_QuandoPedidoForValido()
        {
            // Arrange
            var UserId = Guid.NewGuid();
            var jogoIdReal = Guid.NewGuid();
            var gameResponse = CriarJogoValido(jogoIdReal);

            var command = new PlaceOrderCommand(UserId, "User", "user@teste.com", new List<Guid> { jogoIdReal });

            _gameQueryRepositoryMock
                .Setup(r => r.GetGamesByIdsAsync(command.JogosIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GameResponse> { gameResponse });

            _libraryQueryRepositoryMock
                .Setup(r => r.GetPurchasedGamesByUser(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid>()); // UserLibrary vazia

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            
            _publishEndpointMock.Verify(p => p.Publish(It.Is<OrderPlacedEvent>(e => 
                e.UserId == UserId && 
                e.PrecoTotal == 100.00m && 
                e.JogosIds.Contains(jogoIdReal)), It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogosInexistentes()
        {
            // Arrange
            var UserId = Guid.NewGuid();
            var jogoInexistenteId = Guid.NewGuid();
            
            var command = new PlaceOrderCommand(UserId, "User", "user@teste.com", new List<Guid> { jogoInexistenteId });

            _gameQueryRepositoryMock
                .Setup(r => r.GetGamesByIdsAsync(command.JogosIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GameResponse>()); // Nenhum Game retornado

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
            var GameId = Guid.NewGuid();
            var gameResponse = CriarJogoValido(GameId);
            
            var command = new PlaceOrderCommand(UserId, "User", "user@teste.com", new List<Guid> { GameId });

            _gameQueryRepositoryMock
                .Setup(r => r.GetGamesByIdsAsync(command.JogosIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GameResponse> { gameResponse });

            _libraryQueryRepositoryMock
                .Setup(r => r.GetPurchasedGamesByUser(UserId, It.IsAny<CancellationToken>()))
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
            var GameId = Guid.NewGuid();
            var gameResponse = CriarJogoValido(GameId, isActive: false);
            
            var command = new PlaceOrderCommand(UserId, "User", "user@teste.com", new List<Guid> { GameId });

            _gameQueryRepositoryMock
                .Setup(r => r.GetGamesByIdsAsync(command.JogosIds, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<GameResponse> { gameResponse });

            _libraryQueryRepositoryMock
                .Setup(r => r.GetPurchasedGamesByUser(UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Guid>()); 

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("*não está mais disponível para aquisição.*");
        }
    }
}
