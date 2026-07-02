using Fcg.Catalog.Application.Features.Orders.Commands.PlaceOrder;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Fcg.Catalog.Application.Tests.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandValidatorTests
    {
        private readonly PlaceOrderJogoCommandValidator _validator;

        public PlaceOrderCommandValidatorTests()
        {
            _validator = new PlaceOrderJogoCommandValidator();
        }

        [Fact]
        public void Validar_DeveRetornarErro_QuandoListaDeJogosVazia()
        {
            // Arrange
            var command = new PlaceOrderCommand(Guid.NewGuid(), "User", "user@teste.com", new List<Guid>());

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(e => e.PropertyName == "JogosIds" && e.ErrorMessage == "A lista de Games adquiridos não pode estar vazia.");
        }

        [Fact]
        public void Validar_DevePassar_QuandoListaDeJogosPreenchida()
        {
            // Arrange
            var command = new PlaceOrderCommand(Guid.NewGuid(), "User", "user@teste.com", new List<Guid> { Guid.NewGuid() });

            // Act
            var result = _validator.Validate(command);

            // Assert
            result.IsValid.Should().BeTrue();
        }
    }
}
