using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AtualizarJogo
{
    public class AtualizarJogoCommandValidator : AbstractValidator<AtualizarJogoCommand>
    {
        public AtualizarJogoCommandValidator()
        {
            RuleFor(x => x.JogoId)
                .NotEmpty().WithMessage(MensagensDominio.JogoNaoEncontrado);

            RuleFor(x => x.NovoNome)
                .NotEmpty().WithMessage(MensagensDominio.JogoNomeObrigatorio)
                .Length(3, 20).WithMessage(MensagensDominio.JogoTamanhoNomeInvalido)
                .Matches(@"[a-zA-ZáéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕçÇ]").WithMessage(MensagensDominio.NomeJogoNaoReal);

            RuleFor(x => x.NovaDescricao)
                .NotEmpty().WithMessage(MensagensDominio.JogoDescricaoObrigatoria)
                .Length(5, 100).WithMessage(MensagensDominio.JogoDescricaoTamanhoInvalido)
                .Matches(@"[a-zA-ZáéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕçÇ]").WithMessage(MensagensDominio.DescricaoJogoNaoReal);

            RuleFor(x => x.NovoPreco)
                .GreaterThan(0).WithMessage(MensagensDominio.ValorInvalido);

            RuleFor(x => x.NovoGenero)
                .Must(x => (int)x >= 1 && (int)x <= 20).WithMessage(MensagensDominio.JogoGeneroObrigatorio);
        }
    }
}
