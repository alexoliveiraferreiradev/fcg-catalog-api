using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.ReativarJogo;
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
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Catalog.Commands.Admin.ReativarJogo
{
    public class ReativarJogoCommandHandlerTests
    {
        private readonly Mock<IJogoRepository> _jogoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<ReativarJogoCommandHandler>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly ReativarJogoCommandHandler _handler;

        public ReativarJogoCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IJogoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ReativarJogoCommandHandler>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new ReativarJogoCommandHandler(
                _jogoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mediatorMock.Object
            );
        }

        private Jogo CriarJogoValido()
        {
            return new Jogo(
                new Nome("Jogo Teste"),
                new Descricao("Descricao do jogo longa"),
                new Preco(100.00m),
                GeneroJogo.RPG
            );
        }

        [Fact]
        public async Task Handle_DeveReativarJogoEPublicarEvento_QuandoJogoExistirEEstiverInativo()
        {
            // Arrange
            var jogo = CriarJogoValido();
            jogo.Desativar(); // Deixa inativo primeiro
            var command = new ReativarJogoCommand { JogoId = jogo.Id };

            _jogoRepositoryMock.Setup(r => r.ObterPorId(command.JogoId)).ReturnsAsync(jogo);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            jogo.Ativo.Should().BeTrue();
            _jogoRepositoryMock.Verify(r => r.Atualizar(jogo), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<JogoReativadoEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogoNaoForEncontrado()
        {
            // Arrange
            var command = new ReativarJogoCommand { JogoId = Guid.NewGuid() };
            _jogoRepositoryMock.Setup(r => r.ObterPorId(command.JogoId)).ReturnsAsync((Jogo)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
