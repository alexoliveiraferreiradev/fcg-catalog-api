using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.ObterPromocaoPorJogoId
{
    public record ObterPromocaoPorJogoIdQuery(Guid promocaoId) :IRequest<PromocaoResponse>;
}
