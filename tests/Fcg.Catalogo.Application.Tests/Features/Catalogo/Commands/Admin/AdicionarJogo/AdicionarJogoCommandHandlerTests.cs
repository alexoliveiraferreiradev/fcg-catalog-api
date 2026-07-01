using Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.AdicionarJogo;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Enum;
using Fcg.Catalogo.Domain.Events;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Catalogo.Application.Tests.Features.Catalogo.Commands.Admin.AdicionarJogo
{
    public class AdicionarJogoCommandHandlerTests
    {
        private readonly Mock<IJogoRepository> _jogoRepositoryMock;
        private readonly Mock<ILogger<AdicionarJogoCommandHandler>> _loggerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly AdicionarJogoCommandHandler _handler;

        public AdicionarJogoCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IJogoRepository>();
            _loggerMock = new Mock<ILogger<AdicionarJogoCommandHandler>>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new AdicionarJogoCommandHandler(
                _jogoRepositoryMock.Object,
                _loggerMock.Object,
                _mediatorMock.Object
            );
        }

        [Fact]
        public async Task Handle_DeveAdicionarJogoEPublicarEvento_QuandoNomeEstiverDisponivel()
        {
            // Arrange
            var command = new AdicionarJogoCommand
            {
                Nome = "Novo Jogo",
                Descricao = "Uma descrição super legal para o jogo",
                Preco = 150.0m,
                Genero = GeneroJogo.Estrategia
            };

            _jogoRepositoryMock
                .Setup(r => r.ExisteJogoComNome(command.Nome))
                .ReturnsAsync(false);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().NotBeNull();
            response.Nome.Should().Be(command.Nome);
            response.PrecoOriginal.Should().Be(command.Preco);

            _jogoRepositoryMock.Verify(r => r.Adicionar(It.Is<Jogo>(j => j.Nome.Valor == command.Nome)), Times.Once);
            _mediatorMock.Verify(m => m.Publish(It.IsAny<JogoAdicionadoEvent>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoNomeDoJogoJaExistir()
        {
            // Arrange
            var command = new AdicionarJogoCommand
            {
                Nome = "Jogo Duplicado",
                Descricao = "Descrição",
                Preco = 50.0m,
                Genero = GeneroJogo.Acao
            };

            _jogoRepositoryMock
                .Setup(r => r.ExisteJogoComNome(command.Nome))
                .ReturnsAsync(true); // Nome já existe

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("*Já existe um jogo com esse nome.*");
        }
    }
}
