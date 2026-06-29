using Fcg.Catalogo.Domain.Resources;
using FluentValidation;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.DesativarJogo
{
    public class DesativarJogoCommandValidator : AbstractValidator<DesativarJogoCommand>
    {
        public DesativarJogoCommandValidator()
        {
            RuleFor(x => x.JogoId)
                .NotEmpty().WithMessage(MensagensDominio.JogoNaoEncontrado);
        }
    }
}
