using Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.AtualizarJogo;
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

namespace Fcg.Catalogo.Application.Tests.Features.Catalogo.Commands.Admin.AtualizarJogo
{
    public class AtualizarJogoCommandHandlerTests
    {
        private readonly Mock<IJogoRepository> _jogoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<AtualizarJogoCommandHandler>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AtualizarJogoCommandHandler _handler;

        public AtualizarJogoCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IJogoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AtualizarJogoCommandHandler>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new AtualizarJogoCommandHandler(
                _jogoRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object,
                _mediatorMock.Object
            );
        }

        private Jogo CriarJogoValido()
        {
            return new Jogo(
                new Nome("Jogo Antigo"),
                new Descricao("Descricao antiga longa"),
                new Preco(100.00m),
                GeneroJogo.RPG
            );
        }

        [Fact]
        public async Task Handle_DeveAtualizarJogoEPublicarEvento_QuandoValido()
        {
            // Arrange
            var jogo = CriarJogoValido();
            var command = new AtualizarJogoCommand
            {
                JogoId = jogo.Id,
                NovoNome = "Jogo Novo",
                NovaDescricao = "Nova descricao longa",
                NovoPreco = 120.00m,
                NovoGenero = GeneroJogo.Acao
            };

            _jogoRepositoryMock.Setup(r => r.ObterPorId(command.JogoId)).ReturnsAsync(jogo);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            jogo.Nome.Valor.Should().Be(command.NovoNome);
            jogo.PrecoBase.Valor.Should().Be(command.NovoPreco);

            _jogoRepositoryMock.Verify(r => r.Atualizar(jogo), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<JogoAtualizadoEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogoNaoEncontrado()
        {
            // Arrange
            var command = new AtualizarJogoCommand { JogoId = Guid.NewGuid() };
            _jogoRepositoryMock.Setup(r => r.ObterPorId(command.JogoId)).ReturnsAsync((Jogo)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
