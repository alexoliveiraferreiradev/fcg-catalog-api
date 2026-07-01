using Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.AdicionarPromocaoJogo;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Enum;
using Fcg.Catalogo.Domain.Events;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Catalogo.Application.Tests.Features.Catalogo.Commands.Admin.AdicionarPromocaoJogo
{
    public class AdicionarPromocaoJogoCommandHandlerTests
    {
        private readonly Mock<IJogoRepository> _jogoRepositoryMock;
        private readonly Mock<ILogger<AdicionarPromocaoJogoCommandHandler>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AdicionarPromocaoJogoCommandHandler _handler;

        public AdicionarPromocaoJogoCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IJogoRepository>();
            _loggerMock = new Mock<ILogger<AdicionarPromocaoJogoCommandHandler>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new AdicionarPromocaoJogoCommandHandler(
                _jogoRepositoryMock.Object,
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
        public async Task Handle_DeveAdicionarPromocao_QuandoValido()
        {
            // Arrange
            var jogo = CriarJogoValido();
            var command = new AdicionarPromocaoJogoCommand(jogo.Id, 50.0m, DateTime.UtcNow, DateTime.UtcNow.AddDays(5));

            _jogoRepositoryMock.Setup(r => r.ObterPorId(command.JogoId)).ReturnsAsync(jogo);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.ValorPromocao.Should().Be(command.ValorPromocao);
            
            _jogoRepositoryMock.Verify(r => r.Atualizar(jogo), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<PromocaoAdicionadaEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogoJaPossuirPromocao()
        {
            // Arrange
            var jogo = CriarJogoValido();
            jogo.AdicionarPromocao(new Preco(80.0m), new Periodo(DateTime.UtcNow.AddDays(2))); // Adiciona promo existente
            
            var command = new AdicionarPromocaoJogoCommand(jogo.Id, 50.0m, DateTime.UtcNow, DateTime.UtcNow.AddDays(5));
            _jogoRepositoryMock.Setup(r => r.ObterPorId(command.JogoId)).ReturnsAsync(jogo);

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>();
        }
    }
}
