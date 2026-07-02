using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AdicionarPromocaoJogo
{
    public class AdicionarPromocaoJogoCommandValidator : AbstractValidator<AdicionarPromocaoJogoCommand>
    {
        public AdicionarPromocaoJogoCommandValidator()
        {
            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage(MensagensDominio.JogoNaoEncontrado);

            RuleFor(x => x.ValorPromocao)
                .GreaterThan(0).WithMessage(MensagensDominio.ValorInvalido);

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage(MensagensDominio.DataFimInvalida)
                .GreaterThan(DateTime.UtcNow).WithMessage(MensagensDominio.DataFimInvalida);
        }
    }
}
