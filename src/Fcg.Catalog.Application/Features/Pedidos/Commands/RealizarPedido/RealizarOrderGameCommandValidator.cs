using FluentValidation;

namespace Fcg.Catalog.Application.Features.Orders.Commands.RealizarPedido
{
    public class RealizarPedidoJogoCommandValidator : AbstractValidator<RealizarPedidoCommand>
    {
        public RealizarPedidoJogoCommandValidator()
        {         
            RuleFor(x => x.JogosIds).NotEmpty().WithMessage("A lista de Games adquiridos não pode estar vazia.");
        }
    }
}
