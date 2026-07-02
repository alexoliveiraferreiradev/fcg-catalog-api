using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.UpdatePromotion
{
    public class AtualizarPromocaoCommandValidator : AbstractValidator<AtualizarPromocaoCommand>
    {
        public AtualizarPromocaoCommandValidator()
        {
            RuleFor(x => x.PromotionId)
                .NotEmpty().WithMessage(MensagensDominio.PromocaoNaoEncontrada);

            RuleFor(x => x.GameId)
                .NotEmpty().WithMessage(MensagensDominio.JogoNaoEncontrado);

            RuleFor(x => x.NovoValorPromocao)
                .GreaterThan(0).WithMessage(MensagensDominio.ValorInvalido);

            RuleFor(x => x.NovaDataFim)
                .GreaterThan(DateTime.UtcNow).WithMessage(MensagensDominio.DataFimInvalida);
        }
    }
}
