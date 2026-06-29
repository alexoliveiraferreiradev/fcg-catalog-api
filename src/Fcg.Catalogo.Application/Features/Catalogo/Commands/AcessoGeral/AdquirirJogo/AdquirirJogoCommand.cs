using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.AcessoGeral.AdquirirJogo
{
    public record AdquirirJogoCommand(Guid UsuarioId,IEnumerable<Guid> JogosIds) : IRequest<bool>;
}
