using System;
using System.Threading;
using System.Threading.Tasks;
using Fcg.Catalog.Application.Features.Orders.Commands.FinalizeFailedOrder;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Orders.Commands.FinalizeFailedOrder
{
    public class FinalizeFailedOrderCommandHandlerTests
    {
        private readonly Mock<IOrderRepository> _orderRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<FinalizeFailedOrderCommandHandler>> _loggerMock;
        private readonly FinalizeFailedOrderCommandHandler _handler;

        public FinalizeFailedOrderCommandHandlerTests()
        {
            _orderRepositoryMock = new Mock<IOrderRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<FinalizeFailedOrderCommandHandler>>();

            _handler = new FinalizeFailedOrderCommandHandler(
                _orderRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeveCancelarOrderECommitar_QuandoOrderForEncontrada()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var order = new Order(userId); // Cria order em Draft
            
            var command = new FinalizeFailedCommand(orderId, "Pagamento Recusado");

            _orderRepositoryMock
                .Setup(r => r.GetOrderById(orderId))
                .ReturnsAsync(order);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            order.Status.Should().Be(OrderStatus.Cancelled);
            _orderRepositoryMock.Verify(r => r.Update(order), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarException_QuandoRepositoryFalhar()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var command = new FinalizeFailedCommand(orderId, "Erro no Processamento");

            _orderRepositoryMock
                .Setup(r => r.GetOrderById(orderId))
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Database connection failed");
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Never);
        }
    }
}
