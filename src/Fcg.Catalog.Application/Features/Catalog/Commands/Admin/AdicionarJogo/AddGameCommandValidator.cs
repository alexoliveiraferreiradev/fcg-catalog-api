using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AdicionarJogo
{
    public class AdicionarJogoCommandValidator : AbstractValidator<AddGameCommand>
    {
        public AdicionarJogoCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage(MensagensDominio.JogoNomeObrigatorio)
                .Length(3, 20).WithMessage(MensagensDominio.JogoTamanhoNomeInvalido)
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕçÇ]+$")
                .NotEqual("Name do Game").WithMessage(MensagensDominio.NomeJogoNaoReal);

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage(MensagensDominio.JogoDescricaoObrigatoria)
                .Length(5, 100).WithMessage(MensagensDominio.JogoDescricaoTamanhoInvalido)
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕçÇ]+$")
                .NotEqual("Descrição do Game").WithMessage(MensagensDominio.DescricaoJogoNaoReal);

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage(MensagensDominio.ValorInvalido);

            RuleFor(x => x.Genre)
                .Must(x => (int)x >= 1 && (int)x <= 20).WithMessage(MensagensDominio.JogoGeneroObrigatorio);
        }
    }
}
