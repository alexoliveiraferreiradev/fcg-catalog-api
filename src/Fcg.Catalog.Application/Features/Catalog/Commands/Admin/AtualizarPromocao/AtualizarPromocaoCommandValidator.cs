using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AtualizarPromocao
{
    public class AtualizarPromocaoCommandValidator : AbstractValidator<AtualizarPromocaoCommand>
    {
        public AtualizarPromocaoCommandValidator()
        {
            RuleFor(x => x.PromocaoId)
                .NotEmpty().WithMessage(MensagensDominio.PromocaoNaoEncontrada);

            RuleFor(x => x.JogoId)
                .NotEmpty().WithMessage(MensagensDominio.JogoNaoEncontrado);

            RuleFor(x => x.NovoValorPromocao)
                .GreaterThan(0).WithMessage(MensagensDominio.ValorInvalido);

            RuleFor(x => x.NovaDataFim)
                .GreaterThan(DateTime.UtcNow).WithMessage(MensagensDominio.DataFimInvalida);
        }
    }
}
