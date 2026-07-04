using System;
using Fcg.Catalog.Domain.Entities;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using Xunit;

namespace Fcg.Catalog.Domain.Tests.Entities
{
    public class OrderGameTests
    {
        #region Construtor Tests

        [Fact]
        public void Construtor_ComDadosValidos_DeveCriarOrderGame()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var gameName = "Mega Game";
            var gameAmount = 80.00m;

            // Act
            var orderGame = new OrderGame(orderId, gameId, gameName, gameAmount);

            // Assert
            Assert.Equal(orderId, orderGame.OrderId);
            Assert.Equal(gameId, orderGame.GameId);
            Assert.Equal(gameName, orderGame.GameName);
            Assert.Equal(gameAmount, orderGame.GameAmount);
        }

        [Fact]
        public void Construtor_ComOrderIdVazio_DeveLancarDomainException()
        {
            // Arrange
            var invalidOrderId = Guid.Empty;
            var gameId = Guid.NewGuid();
            var gameName = "Mega Game";
            var gameAmount = 80.00m;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => 
                new OrderGame(invalidOrderId, gameId, gameName, gameAmount));
            Assert.Equal(DomainMessages.OrderWithoutUser, exception.Message);
        }

        [Fact]
        public void Construtor_ComGameIdVazio_DeveLancarDomainException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var invalidGameId = Guid.Empty;
            var gameName = "Mega Game";
            var gameAmount = 80.00m;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => 
                new OrderGame(orderId, invalidGameId, gameName, gameAmount));
            Assert.Equal(DomainMessages.GameNotFound, exception.Message);
        }

        [Fact]
        public void Construtor_ComGameNameNulo_DeveLancarDomainException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            string gameNameNulo = null!;
            var gameAmount = 80.00m;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => 
                new OrderGame(orderId, gameId, gameNameNulo, gameAmount));
            Assert.Equal(DomainMessages.GameNameRequired, exception.Message);
        }

        [Fact]
        public void Construtor_ComValorNegativo_DeveLancarDomainException()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var gameId = Guid.NewGuid();
            var gameName = "Mega Game";
            var negativeAmount = -10.00m;

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => 
                new OrderGame(orderId, gameId, gameName, negativeAmount));
            Assert.Equal("O Valor do item não pode ser negativo", exception.Message);
        }

        #endregion
    }
}
