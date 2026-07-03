using FluentValidation;

namespace Fcg.Catalog.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderJogoCommandValidator : AbstractValidator<PlaceOrderCommand>
    {
        public PlaceOrderJogoCommandValidator()
        {         
            RuleFor(x => x.JogosIds).NotEmpty().WithMessage("A lista de Games adquiridos não pode estar vazia.");
        }
    }
}
