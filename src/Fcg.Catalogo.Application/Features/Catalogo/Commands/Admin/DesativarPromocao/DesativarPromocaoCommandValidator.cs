using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.DesativarPromocao
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
