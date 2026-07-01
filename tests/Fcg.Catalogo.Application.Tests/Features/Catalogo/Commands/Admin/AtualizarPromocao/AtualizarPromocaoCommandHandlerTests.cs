using Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.AtualizarPromocao;
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Catalogo.Application.Tests.Features.Catalogo.Commands.Admin.AtualizarPromocao
{
    public class AtualizarPromocaoCommandHandlerTests
    {
        private readonly Mock<IJogoRepository> _jogoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<AtualizarPromocaoCommandHandler>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AtualizarPromocaoCommandHandler _handler;

        public AtualizarPromocaoCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IJogoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AtualizarPromocaoCommandHandler>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new AtualizarPromocaoCommandHandler(
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
        public async Task Handle_DeveAtualizarPromocao_QuandoValido()
        {
            // Arrange
            var jogo = CriarJogoValido();
            jogo.AdicionarPromocao(new Preco(80.0m), new Periodo(DateTime.UtcNow.AddDays(5)));
            var promocao = jogo.Promocoes.First();

            var command = new AtualizarPromocaoCommand
            {
                PromocaoId = promocao.Id,
                JogoId = jogo.Id,
                NovoValorPromocao = 60.0m,
                NovaDataFim = DateTime.UtcNow.AddDays(10)
            };

            _jogoRepositoryMock.Setup(r => r.ObterPorId(command.JogoId)).ReturnsAsync(jogo);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            promocao.ValorPromocao.Valor.Should().Be(command.NovoValorPromocao);
            _jogoRepositoryMock.Verify(r => r.Atualizar(jogo), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<PromocaoAtualizadaEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogoNaoEncontrado()
        {
            // Arrange
            var command = new AtualizarPromocaoCommand { JogoId = Guid.NewGuid() };
            _jogoRepositoryMock.Setup(r => r.ObterPorId(command.JogoId)).ReturnsAsync((Jogo)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
