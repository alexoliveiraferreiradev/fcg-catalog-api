using Fcg.Catalogo.Domain.Resources;
using FluentValidation;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.ReativarJogo
{
    public class ReativarJogoCommandValidator : AbstractValidator<ReativarJogoCommand>
    {
        public ReativarJogoCommandValidator()
        {
            RuleFor(x => x.JogoId)
                .NotEmpty().WithMessage(MensagensDominio.JogoNaoEncontrado);
        }
    }
}
