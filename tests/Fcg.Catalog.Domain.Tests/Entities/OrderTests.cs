using System;
using System.Linq;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using Xunit;

namespace Fcg.Catalog.Domain.Tests.Entities
{
    public class OrderTests
    {
        #region Construtor Tests

        [Fact]
        public void Construtor_ComUsuarioValido_DeveCriarOrderComStatusDraft()
        {
            // Arrange
            var userId = Guid.NewGuid();

            // Act
            var order = new Order(userId);

            // Assert
            Assert.Equal(userId, order.UserId);
            Assert.Equal(OrderStatus.Draft, order.Status);
            Assert.Empty(order.Games);
            Assert.Equal(0, order.TotalAmount.Amount);
            Assert.True((DateTime.UtcNow - order.CreatedAt).TotalSeconds < 5);
            Assert.Equal(order.CreatedAt, order.UpdatedAt);
        }

        [Fact]
        public void Construtor_ComUsuarioInvalido_DeveLancarDomainException()
        {
            // Arrange
            var invalidUserId = Guid.Empty;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => new Order(invalidUserId));
            Assert.Equal(DomainMessages.OrderWithoutUser, exception.Message);
        }

        #endregion

        #region AddItem Tests

        [Fact]
        public void AddItem_ComDadosValidos_DeveAdicionarItem()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var gameId = Guid.NewGuid();
            var gameName = "Super Game";
            var gameAmount = 150.00m;

            // Act
            order.AddItem(gameId, gameName, gameAmount);

            // Assert
            Assert.Single(order.Games);
            var item = order.Games.First();
            Assert.Equal(gameId, item.GameId);
            Assert.Equal(gameName, item.GameName);
            Assert.Equal(gameAmount, item.GameAmount);
        }

        [Fact]
        public void AddItem_QuandoStatusNaoForDraft_DeveLancarDomainException()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.AddItem(Guid.NewGuid(), "Game 1", 100m);
            order.FinalizeOrder(); // Altera status para Completed

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => 
                order.AddItem(Guid.NewGuid(), "Game 2", 150m));
            Assert.Equal(DomainMessages.OrderGameNotDraft, exception.Message);
        }

        [Fact]
        public void AddItem_ComJogoIdVazio_DeveLancarDomainException()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => 
                order.AddItem(Guid.Empty, "Game 1", 100m));
            Assert.Equal(DomainMessages.GameNotFound, exception.Message);
        }

        [Fact]
        public void AddItem_ComJogoJaAdicionado_DeveLancarDomainException()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            var gameId = Guid.NewGuid();
            order.AddItem(gameId, "Game 1", 100m);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => 
                order.AddItem(gameId, "Game 1 Duplicado", 100m));
            Assert.Equal(DomainMessages.OrderGameAlreadyAdded, exception.Message);
        }

        #endregion

        #region CancelOrder Tests

        [Fact]
        public void CancelOrder_QuandoDraft_DeveCancelar()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            // Act
            order.CancelOrder();

            // Assert
            Assert.Equal(OrderStatus.Cancelled, order.Status);
            Assert.True((DateTime.UtcNow - order.UpdatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void CancelOrder_QuandoNaoDraft_DeveLancarDomainException()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.AddItem(Guid.NewGuid(), "Game 1", 100m);
            order.FinalizeOrder();

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => order.CancelOrder());
            Assert.Equal(DomainMessages.OrderNotDraft, exception.Message);
        }

        #endregion

        #region FinalizeOrder Tests

        [Fact]
        public void FinalizeOrder_ComItens_DeveFinalizarECalcularTotal()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.AddItem(Guid.NewGuid(), "Game 1", 100.00m);
            order.AddItem(Guid.NewGuid(), "Game 2", 150.00m);

            // Act
            order.FinalizeOrder();

            // Assert
            Assert.Equal(OrderStatus.Completed, order.Status);
            Assert.Equal(250.00m, order.TotalAmount.Amount);
            Assert.True((DateTime.UtcNow - order.UpdatedAt).TotalSeconds < 5);
        }

        [Fact]
        public void FinalizeOrder_SemItens_DeveLancarDomainException()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => order.FinalizeOrder());
            Assert.Equal(DomainMessages.OrderWithoutGames, exception.Message);
        }

        [Fact]
        public void FinalizeOrder_QuandoNaoDraft_DeveLancarDomainException()
        {
            // Arrange
            var order = new Order(Guid.NewGuid());
            order.AddItem(Guid.NewGuid(), "Game 1", 100m);
            order.FinalizeOrder(); // Já finalizado

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => order.FinalizeOrder());
            Assert.Equal(DomainMessages.OrderNotDraft, exception.Message);
        }

        #endregion
    }
}
