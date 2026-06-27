using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.DesativarPromocao
{
    public record DesativarPromocaoCommand : IRequest
    {
        public Guid PromocaoId { get; init; }
    }
}
