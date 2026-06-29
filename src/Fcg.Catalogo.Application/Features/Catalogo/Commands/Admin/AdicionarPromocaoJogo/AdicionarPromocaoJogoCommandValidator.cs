using Fcg.Catalogo.Domain.Resources;
using FluentValidation;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.AdicionarPromocaoJogo
{
    public class AdicionarPromocaoJogoCommandValidator : AbstractValidator<AdicionarPromocaoJogoCommand>
    {
        public AdicionarPromocaoJogoCommandValidator()
        {
            RuleFor(x => x.JogoId)
                .NotEmpty().WithMessage(MensagensDominio.JogoNaoEncontrado);

            RuleFor(x => x.ValorPromocao)
                .GreaterThan(0).WithMessage(MensagensDominio.ValorInvalido);

            RuleFor(x => x.DataFim)
                .GreaterThan(x => x.DataInicio).WithMessage(MensagensDominio.DataFimInvalida)
                .GreaterThan(DateTime.UtcNow).WithMessage(MensagensDominio.DataFimInvalida);
        }
    }
}
