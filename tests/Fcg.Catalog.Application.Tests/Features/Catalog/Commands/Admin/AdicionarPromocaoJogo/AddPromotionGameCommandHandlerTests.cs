using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AdicionarPromocaoJogo;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Catalog.Commands.Admin.AdicionarPromocaoJogo
{
    public class AdicionarPromocaoJogoCommandHandlerTests
    {
        private readonly Mock<IGameRepository> _jogoRepositoryMock;
        private readonly Mock<ILogger<AdicionarPromocaoJogoCommandHandler>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AdicionarPromocaoJogoCommandHandler _handler;

        public AdicionarPromocaoJogoCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IGameRepository>();
            _loggerMock = new Mock<ILogger<AdicionarPromocaoJogoCommandHandler>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new AdicionarPromocaoJogoCommandHandler(
                _jogoRepositoryMock.Object,
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
        public async Task Handle_DeveAdicionarPromocao_QuandoValido()
        {
            // Arrange
            var Game = CriarJogoValido();
            var command = new AdicionarPromocaoJogoCommand(Game.Id, 50.0m, DateTime.UtcNow, DateTime.UtcNow.AddDays(5));

            _jogoRepositoryMock.Setup(r => r.GetById(command.GameId)).ReturnsAsync(Game);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.ValorPromocao.Should().Be(command.ValorPromocao);
            
            _jogoRepositoryMock.Verify(r => r.Update(Game), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<PromotionAddedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogoJaPossuirPromocao()
        {
            // Arrange
            var Game = CriarJogoValido();
            Game.AddPromotion(new Price(80.0m), new Period(DateTime.UtcNow.AddDays(2))); // Adiciona promo existente
            
            var command = new AdicionarPromocaoJogoCommand(Game.Id, 50.0m, DateTime.UtcNow, DateTime.UtcNow.AddDays(5));
            _jogoRepositoryMock.Setup(r => r.GetById(command.GameId)).ReturnsAsync(Game);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
