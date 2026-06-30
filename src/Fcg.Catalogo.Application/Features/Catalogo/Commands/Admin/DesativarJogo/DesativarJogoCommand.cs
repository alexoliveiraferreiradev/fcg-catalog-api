using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.DesativarJogo
{
    public record DesativarJogoCommand(Guid JogoId) : IRequest;
}
