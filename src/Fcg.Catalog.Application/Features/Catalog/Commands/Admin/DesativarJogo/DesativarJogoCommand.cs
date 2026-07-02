using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DesativarJogo
{
    public record DesativarJogoCommand(Guid JogoId) : IRequest;
}
