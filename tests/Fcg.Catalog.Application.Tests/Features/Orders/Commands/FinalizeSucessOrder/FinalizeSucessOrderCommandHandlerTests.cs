using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fcg.Catalog.Application.Features.Orders.Commands.FinalizeSucessOrder;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Enum;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using FluentAssertions;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Orders.Commands.FinalizeSucessOrder
{
    public class FinalizeSucessOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<FinalizeSucessOrderCommandHandler>> _loggerMock;
        private readonly Mock<ILibraryRepository> _libraryRepositoryMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly FinalizeSucessOrderCommandHandler _handler;

        public FinalizeSucessOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<FinalizeSucessOrderCommandHandler>>();
            _libraryRepositoryMock = new Mock<ILibraryRepository>();
            _mediatorMock = new Mock<IMediator>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();

            _handler = new FinalizeSucessOrderCommandHandler(
                _orderRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _libraryRepositoryMock.Object,
                _mediatorMock.Object,
                _publishEndpointMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeveAdicionarJogosNaBibliotecaFinalizarOrderECommitar_QuandoSucesso()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var gameId1 = Guid.NewGuid();
            var gameId2 = Guid.NewGuid();

            var order = new Order(userId);
            order.AddItem(gameId1, "Game 1", 50m);
            order.AddItem(gameId2, "Game 2", 100m);

            var command = new FinalizeSucessOrderCommand(
                orderId,
                userId,
                new List<Guid> { gameId1, gameId2 }
            );

            _orderRepositoryMock
                .Setup(r => r.GetOrderById(orderId))
                .ReturnsAsync(order);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            order.Status.Should().Be(OrderStatus.Completed);
            order.TotalAmount.Amount.Should().Be(150m);

            _libraryRepositoryMock.Verify(r => r.Add(It.Is<UserLibrary>(l => l.UserId == userId && l.GameId == gameId1)), Times.Once);
            _libraryRepositoryMock.Verify(r => r.Add(It.Is<UserLibrary>(l => l.UserId == userId && l.GameId == gameId2)), Times.Once);
            
            _mediatorMock.Verify(m => m.Publish(It.Is<LibraryEvent>(e => e.UserId == userId), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _publishEndpointMock.Verify(p => p.Publish(It.IsAny<DeliveryFailedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_DevePublicarDeliveryFailedEvent_QuandoOcorrerAlgumaFalha()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new FinalizeSucessOrderCommand(
                orderId,
                userId,
                new List<Guid> { Guid.NewGuid() }
            );

            // Forçar falha jogando uma exceção ao buscar a order
            _orderRepositoryMock
                .Setup(r => r.GetOrderById(orderId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _publishEndpointMock.Verify(p => p.Publish(It.Is<DeliveryFailedEvent>(e => 
                e.OrderId == orderId && 
                e.UserId == userId && 
                e.Reason == "Falha ao finalizar a Order e adicionar os Games à Library do Usuário."), 
                It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
