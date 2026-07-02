using Fcg.Core.Abstractions.Resources;
using FluentValidation;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AdicionarJogo
{
    public class AdicionarJogoCommandValidator : AbstractValidator<AdicionarJogoCommand>
    {
        public AdicionarJogoCommandValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage(MensagensDominio.JogoNomeObrigatorio)
                .Length(3, 20).WithMessage(MensagensDominio.JogoTamanhoNomeInvalido)
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕçÇ]+$")
                .NotEqual("Nome do Jogo").WithMessage(MensagensDominio.NomeJogoNaoReal);

            RuleFor(x => x.Descricao)
                .NotEmpty().WithMessage(MensagensDominio.JogoDescricaoObrigatoria)
                .Length(5, 100).WithMessage(MensagensDominio.JogoDescricaoTamanhoInvalido)
                .Matches(@"^[a-zA-ZáéíóúÁÉÍÓÚâêîôûÂÊÎÔÛãõÃÕçÇ]+$")
                .NotEqual("Descrição do Jogo").WithMessage(MensagensDominio.DescricaoJogoNaoReal);

            RuleFor(x => x.Preco)
                .GreaterThan(0).WithMessage(MensagensDominio.ValorInvalido);

            RuleFor(x => x.Genero)
                .Must(x => (int)x >= 1 && (int)x <= 20).WithMessage(MensagensDominio.JogoGeneroObrigatorio);
        }
    }
}
