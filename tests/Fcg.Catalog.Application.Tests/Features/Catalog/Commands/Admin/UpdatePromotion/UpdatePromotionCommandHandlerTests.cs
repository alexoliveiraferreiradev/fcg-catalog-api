using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.UpdatePromotion;
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Catalog.Commands.Admin.UpdatePromotion
{
    public class UpdatePromotionCommandHandlerTests
    {
        private readonly Mock<IGameRepository> _jogoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<UpdatePromotionCommandHandler>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly UpdatePromotionCommandHandler _handler;

        public UpdatePromotionCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IGameRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<UpdatePromotionCommandHandler>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new UpdatePromotionCommandHandler(
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
        public async Task Handle_DeveUpdatePromotion_QuandoValido()
        {
            // Arrange
            var Game = CriarJogoValido();
            Game.AddPromotion(new Price(80.0m), new Period(DateTime.UtcNow.AddDays(5)));
            var Promotion = Game.Promotions.First();

            var command = new UpdatePromotionCommand
            {
                PromotionId = Promotion.Id,
                GameId = Game.Id,
                NovoValorPromocao = 60.0m,
                NovaDataFim = DateTime.UtcNow.AddDays(10)
            };

            _jogoRepositoryMock.Setup(r => r.GetById(command.GameId)).ReturnsAsync(Game);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            Promotion.ValorPromocao.Amount.Should().Be(command.NovoValorPromocao);
            _jogoRepositoryMock.Verify(r => r.Update(Game), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<PromotionUpdatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogoNaoEncontrado()
        {
            // Arrange
            var command = new UpdatePromotionCommand { GameId = Guid.NewGuid() };
            _jogoRepositoryMock.Setup(r => r.GetById(command.GameId)).ReturnsAsync((Game)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
