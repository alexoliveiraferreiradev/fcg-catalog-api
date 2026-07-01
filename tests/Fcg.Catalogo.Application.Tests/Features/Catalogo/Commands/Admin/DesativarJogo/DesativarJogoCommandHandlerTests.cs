using Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.DesativarJogo;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Enum;
using Fcg.Catalogo.Domain.Events;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Domain.ValueObject;
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

namespace Fcg.Catalogo.Application.Tests.Features.Catalogo.Commands.Admin.DesativarJogo
{
    public class DesativarJogoCommandHandlerTests
    {
        private readonly Mock<IJogoRepository> _jogoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<DesativarJogoCommandHandler>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DesativarJogoCommandHandler _handler;

        public DesativarJogoCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IJogoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DesativarJogoCommandHandler>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new DesativarJogoCommandHandler(
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
        public async Task Handle_DeveDesativarJogoEPublicarEvento_QuandoJogoExistirEEstiverAtivo()
        {
            // Arrange
            var jogo = CriarJogoValido();
            var command = new DesativarJogoCommand(jogo.Id);

            _jogoRepositoryMock.Setup(r => r.ObterPorId(command.JogoId)).ReturnsAsync(jogo);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            jogo.Ativo.Should().BeFalse();
            _jogoRepositoryMock.Verify(r => r.Atualizar(jogo), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<JogoDesativadoEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogoNaoForEncontrado()
        {
            // Arrange
            var command = new DesativarJogoCommand(Guid.NewGuid());
            _jogoRepositoryMock.Setup(r => r.ObterPorId(command.JogoId)).ReturnsAsync((Jogo)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
