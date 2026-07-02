using Bogus;
using Fcg.Catalog.Application.Features.Orders.Commands.PlaceOrder;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
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
    public class RealizarPedidoCommandHandlerTests
    {
        private readonly Mock<IGameRepository> _jogoRepositoryMock;
        private readonly Mock<ILibraryRepository> _bibliotecaRepositoryMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<RealizarPedidoCommandHandler>> _loggerMock;
        private readonly RealizarPedidoCommandHandler _handler;

        public RealizarPedidoCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IGameRepository>();
            _bibliotecaRepositoryMock = new Mock<ILibraryRepository>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<RealizarPedidoCommandHandler>>();

            _handler = new RealizarPedidoCommandHandler(
                _jogoRepositoryMock.Object,
                _publishEndpointMock.Object,
                _bibliotecaRepositoryMock.Object,
                _unitOfWorkMock.Object,
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
            var GameId = Guid.NewGuid();
            var UserId = Guid.NewGuid();
            
            var Game = CriarJogoValido();
            // Para "setar" a propriedade Id que tem private set da AggregateRoot (caso precise do reflection ou mock). 
            // Vamos assumir que na comparação vai pelo ID e o ID gerado pelo Construtor base AggregateRoot (que gera um novo Guid)
            var jogoIdReal = Game.Id;

            var command = new RealizarPedidoCommand(UserId, "User", "user@teste.com", new List<Guid> { jogoIdReal });

            _jogoRepositoryMock
                .Setup(r => r.GetGamesByIds(command.JogosIds))
                .ReturnsAsync(new List<Game> { Game });

            _bibliotecaRepositoryMock
                .Setup(r => r.GetPurchasedGamesByUser(UserId))
                .ReturnsAsync(new List<Guid>()); // Library vazia

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
            
            var command = new RealizarPedidoCommand(UserId, "User", "user@teste.com", new List<Guid> { jogoInexistenteId });

            _jogoRepositoryMock
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
            var Game = CriarJogoValido();
            var GameId = Game.Id;
            
            var command = new RealizarPedidoCommand(UserId, "User", "user@teste.com", new List<Guid> { GameId });

            _jogoRepositoryMock
                .Setup(r => r.GetGamesByIds(command.JogosIds))
                .ReturnsAsync(new List<Game> { Game });

            _bibliotecaRepositoryMock
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
            var Game = CriarJogoValido();
            Game.Deactivate();
            var GameId = Game.Id;
            
            var command = new RealizarPedidoCommand(UserId, "User", "user@teste.com", new List<Guid> { GameId });

            _jogoRepositoryMock
                .Setup(r => r.GetGamesByIds(command.JogosIds))
                .ReturnsAsync(new List<Game> { Game });

            _bibliotecaRepositoryMock
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
