using FluentValidation;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.AcessoGeral.AdquirirJogo
{
    public class AdquirirJogoCommandValidator : AbstractValidator<AdquirirJogoCommand>
    {
        public AdquirirJogoCommandValidator()
        {
            RuleFor(x => x.UsuarioId).NotEmpty().WithMessage("ID do usuário é obrigatório.");
            RuleFor(x => x.JogosIds).NotEmpty().WithMessage("A lista de jogos adquiridos não pode estar vazia.");
        }
    }
}
