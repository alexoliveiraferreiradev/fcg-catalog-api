using FluentValidation;

namespace Fcg.Catalogo.Application.Features.Pedidos.Commands.RealizarPedido
{
    public class RealizarPedidoJogoCommandValidator : AbstractValidator<RealizarPedidoCommand>
    {
        public RealizarPedidoJogoCommandValidator()
        {         
            RuleFor(x => x.JogosIds).NotEmpty().WithMessage("A lista de jogos adquiridos não pode estar vazia.");
        }
    }
}
