using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.User.SearchGames
{
    /// <summary>Busca jogos ativos no Elasticsearch, com tolerância a erro de digitação e ordem por relevância.</summary>
    public record SearchGamesQuery(string Term, int Page = 1, int PageSize = 10) : IRequest<PagedResult<GameUserResponse>>
    {
        public const int MaxTermLength = 100;
        public const int MaxPageSize = 50;       
        public const int MaxResultWindow = 10_000;
    }
}
