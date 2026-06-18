using Fcg.Catalogo.Application.Features.Response;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Jogos.Queries.ObterPromocaoPorJogoId
{
    public record ObterPromocaoPorJogoIdQuery(Guid promocaoId) :IRequest<PromocaoResponse>;
}
