using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.DesativarPromocao
{
    public record DesativarPromocaoCommand : IRequest
    {
        public Guid PromocaoId { get; init; }
    }
}
