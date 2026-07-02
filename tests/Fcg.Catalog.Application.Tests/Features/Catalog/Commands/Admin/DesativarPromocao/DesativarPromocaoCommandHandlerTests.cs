using Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DesativarPromocao;
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

namespace Fcg.Catalog.Application.Tests.Features.Catalog.Commands.Admin.DesativarPromocao
{
    public class DesativarPromocaoCommandHandlerTests
    {
        private readonly Mock<IJogoRepository> _jogoRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<DesativarPromocaoCommandHandler>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly DesativarPromocaoCommandHandler _handler;

        public DesativarPromocaoCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IJogoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<DesativarPromocaoCommandHandler>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new DesativarPromocaoCommandHandler(
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
        public async Task Handle_DeveDesativarPromocao_QuandoExistirParaOJogo()
        {
            // Arrange
            var jogo = CriarJogoValido();
            jogo.AdicionarPromocao(new Preco(50.0m), new Periodo(DateTime.UtcNow.AddDays(10)));
            var promocao = jogo.Promocoes.First();

            var command = new DesativarPromocaoCommand { PromocaoId = promocao.Id };

            _jogoRepositoryMock.Setup(r => r.ObterPromocaoPorId(command.PromocaoId)).ReturnsAsync(promocao);
            _jogoRepositoryMock.Setup(r => r.ObterPorId(jogo.Id)).ReturnsAsync(jogo);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            promocao.Ativo.Should().BeFalse();
            _jogoRepositoryMock.Verify(r => r.Atualizar(jogo), Times.Once);
            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<PromocaoDesativadaEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoPromocaoNaoForEncontrada()
        {
            // Arrange
            var command = new DesativarPromocaoCommand { PromocaoId = Guid.NewGuid() };
            _jogoRepositoryMock.Setup(r => r.ObterPromocaoPorId(command.PromocaoId)).ReturnsAsync((Promocao)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
