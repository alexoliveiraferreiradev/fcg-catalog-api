using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetPagedCatalog
{
    public class GetPagedCatalogQueryHandler : IRequestHandler<GetPagedCatalogQuery, PagedResult<GameUserResponse>>
    {        
        private readonly ICacheService _cacheService;
        private readonly IGameQueryRepository _gameQueryRepository;

        public GetPagedCatalogQueryHandler(
            ICacheService cacheService,
            IGameQueryRepository gameQueryRepository)
        {        
            _cacheService = cacheService;
            _gameQueryRepository = gameQueryRepository;
        }

        public async Task<PagedResult<GameUserResponse>> Handle(GetPagedCatalogQuery request, CancellationToken cancellationToken)
        {
            string g = request.Genre.HasValue ? request.Genre.Value.ToString() : "genres";
            string p = request.OnlyPromoted.GetValueOrDefault() ? "yes" : "no";

            var cacheKey = $"catalog:pag:p{request.Page}:t{request.PageSize}:g_{g}:prom_{p}";

            var cachedCatalog = await _cacheService.GetAsync<PagedResult<GameUserResponse>>(cacheKey, cancellationToken);

            if (cachedCatalog != null)
            {
                return cachedCatalog;
            }

            var pagedCatalog = await _gameQueryRepository.GetPagedCatalogAsync(request.Genre, request.OnlyPromoted,request.Page,request.PageSize, cancellationToken);


            if (pagedCatalog.Items.Any())
            {
                await _cacheService.SetAsync(cacheKey, pagedCatalog, TimeSpan.FromMinutes(5), cancellationToken);
            }


            return pagedCatalog;
        }
    }
}
