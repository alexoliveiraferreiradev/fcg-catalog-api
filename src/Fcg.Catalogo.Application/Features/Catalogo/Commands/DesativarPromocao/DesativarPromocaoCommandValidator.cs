using Fcg.Catalogo.Domain.Resources;
using FluentValidation;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.DesativarPromocao
{
    public class DesativarPromocaoCommandValidator : AbstractValidator<DesativarPromocaoCommand>
    {
        public DesativarPromocaoCommandValidator()
        {
            RuleFor(x => x.PromocaoId)
                .NotEmpty().WithMessage(MensagensDominio.PromocaoNaoEncontrada);
        }
    }
}
