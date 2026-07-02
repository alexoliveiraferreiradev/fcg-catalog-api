using Bogus;
using Fcg.Catalog.Application.Features.Pedidos.Commands.RealizarPedido;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Pedidos.Commands.RealizarPedido
{
    public class RealizarPedidoCommandHandlerTests
    {
        private readonly Mock<IJogoRepository> _jogoRepositoryMock;
        private readonly Mock<IBibliotecaRepository> _bibliotecaRepositoryMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<RealizarPedidoCommandHandler>> _loggerMock;
        private readonly RealizarPedidoCommandHandler _handler;

        public RealizarPedidoCommandHandlerTests()
        {
            _jogoRepositoryMock = new Mock<IJogoRepository>();
            _bibliotecaRepositoryMock = new Mock<IBibliotecaRepository>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<RealizarPedidoCommandHandler>>();

            _handler = new RealizarPedidoCommandHandler(
                _jogoRepositoryMock.Object,
                _publishEndpointMock.Object,
                _bibliotecaRepositoryMock.Object,
                _unitOfWorkMock.Object,
                _loggerMock.Object
            );
        }

        private Jogo CriarJogoValido()
        {
            return new Jogo(
                new Nome("Jogo Teste"),
                new Descricao("Descricao do Jogo Teste longo para passar na validacao"),
                new Preco(100.00m),
                GeneroJogo.RPG
            );
        }

        [Fact]
        public async Task Handle_DevePublicarEventoERetornarTrue_QuandoPedidoForValido()
        {
            // Arrange
            var jogoId = Guid.NewGuid();
            var usuarioId = Guid.NewGuid();
            
            var jogo = CriarJogoValido();
            // Para "setar" a propriedade Id que tem private set da AggregateRoot (caso precise do reflection ou mock). 
            // Vamos assumir que na comparação vai pelo ID e o ID gerado pelo Construtor base AggregateRoot (que gera um novo Guid)
            var jogoIdReal = jogo.Id;

            var command = new RealizarPedidoCommand(usuarioId, "User", "user@teste.com", new List<Guid> { jogoIdReal });

            _jogoRepositoryMock
                .Setup(r => r.ObterJogosPorIds(command.JogosIds))
                .ReturnsAsync(new List<Jogo> { jogo });

            _bibliotecaRepositoryMock
                .Setup(r => r.ObterJogosAdquiridosPorUsuario(usuarioId))
                .ReturnsAsync(new List<Guid>()); // Biblioteca vazia

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().BeTrue();
            
            _publishEndpointMock.Verify(p => p.Publish(It.Is<OrderPlacedEvent>(e => 
                e.UserId == usuarioId && 
                e.PrecoTotal == 100.00m && 
                e.JogosIds.Contains(jogoIdReal)), It.IsAny<CancellationToken>()), Times.Once);

            _unitOfWorkMock.Verify(u => u.CommitAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogosInexistentes()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var jogoInexistenteId = Guid.NewGuid();
            
            var command = new RealizarPedidoCommand(usuarioId, "User", "user@teste.com", new List<Guid> { jogoInexistenteId });

            _jogoRepositoryMock
                .Setup(r => r.ObterJogosPorIds(command.JogosIds))
                .ReturnsAsync(new List<Jogo>()); // Nenhum jogo retornado

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("*A compra foi cancelada. Os seguintes IDs de jogos não foram encontrados no catálogo*");
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoUsuarioJaPossuirOJogo()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var jogo = CriarJogoValido();
            var jogoId = jogo.Id;
            
            var command = new RealizarPedidoCommand(usuarioId, "User", "user@teste.com", new List<Guid> { jogoId });

            _jogoRepositoryMock
                .Setup(r => r.ObterJogosPorIds(command.JogosIds))
                .ReturnsAsync(new List<Jogo> { jogo });

            _bibliotecaRepositoryMock
                .Setup(r => r.ObterJogosAdquiridosPorUsuario(usuarioId))
                .ReturnsAsync(new List<Guid> { jogoId }); // Usuário já possui este jogo

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("*já possui o jogo*");
        }

        [Fact]
        public async Task Handle_DeveLancarDomainException_QuandoJogoEstiverInativo()
        {
            // Arrange
            var usuarioId = Guid.NewGuid();
            var jogo = CriarJogoValido();
            jogo.Desativar();
            var jogoId = jogo.Id;
            
            var command = new RealizarPedidoCommand(usuarioId, "User", "user@teste.com", new List<Guid> { jogoId });

            _jogoRepositoryMock
                .Setup(r => r.ObterJogosPorIds(command.JogosIds))
                .ReturnsAsync(new List<Jogo> { jogo });

            _bibliotecaRepositoryMock
                .Setup(r => r.ObterJogosAdquiridosPorUsuario(usuarioId))
                .ReturnsAsync(new List<Guid>()); 

            // Act
            Func<Task> act = async () => await _handler.Handle(command, CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("*não está mais disponível para aquisição.*");
        }
    }
}
