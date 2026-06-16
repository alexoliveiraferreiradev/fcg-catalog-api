using Fcg.Catalogo.Application.Dtos.Promocao;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Jogos.Queries.ObterPromocaoPorJogoId
{
    public record ObterPromocaoPorJogoIdQuery(Guid promocaoId) :IRequest<PromocaoResponse>;
}
