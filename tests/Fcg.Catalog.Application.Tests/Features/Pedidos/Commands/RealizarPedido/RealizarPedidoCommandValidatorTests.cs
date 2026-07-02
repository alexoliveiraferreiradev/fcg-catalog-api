using Fcg.Catalog.Application.Features.Pedidos.Commands.RealizarPedido;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Pedidos.Commands.RealizarPedido
{
    public class RealizarPedidoCommandValidatorTests
    {
        private readonly RealizarPedidoJogoCommandValidator _validator;

        public RealizarPedidoCommandValidatorTests()
        {
            _validator = new RealizarPedidoJogoCommandValidator();
        }

        [Fact]
        public void Validar_DeveRetornarErro_QuandoListaDeJogosVazia()
        {
            // Arrange
            var command = new RealizarPedidoCommand(Guid.NewGuid(), "User", "user@teste.com", new List<Guid>());

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "JogosIds" && e.ErrorMessage == "A lista de jogos adquiridos não pode estar vazia.");
        }

        [Fact]
        public void Validar_DevePassar_QuandoListaDeJogosPreenchida()
        {
            // Arrange
            var command = new RealizarPedidoCommand(Guid.NewGuid(), "User", "user@teste.com", new List<Guid> { Guid.NewGuid() });

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
