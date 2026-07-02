using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DesativarPromocao
{
    public record DesativarPromocaoCommand : IRequest
    {
        public Guid PromocaoId { get; init; }
    }
}
