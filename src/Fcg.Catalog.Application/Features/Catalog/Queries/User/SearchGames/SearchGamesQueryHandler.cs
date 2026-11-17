using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.User.SearchGames
{
    public class SearchGamesQueryHandler : IRequestHandler<SearchGamesQuery, PagedResult<GameUserResponse>>
    {
        private readonly IGameSearchService _searchService;

        public SearchGamesQueryHandler(IGameSearchService searchService)
        {
            _searchService = searchService;
        }

        // Sem cache: o Elasticsearch já é o caminho rápido, e o termo livre quase não repete chave.
        public Task<PagedResult<GameUserResponse>> Handle(SearchGamesQuery request, CancellationToken cancellationToken) =>
            _searchService.SearchAsync(request.Term.Trim(), request.Page, request.PageSize, cancellationToken);
    }
}
