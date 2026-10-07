using Dapper;
using Fcg.Catalog.Application.Common;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.User.GetPagedPromotedCatalogGames
{
    public class GetPagedPromotedCatalogGamesQueryHandler : IRequestHandler<GetPagedPromotedCatalogGamesQuery, PagedResult<GamePromotionResponse>>
    {
        private readonly ICacheService _cacheService;
        private readonly IPromotionQueryRepository _promotionQueryRepository;
        public GetPagedPromotedCatalogGamesQueryHandler(ICacheService cacheService,
            IPromotionQueryRepository promotionQueryRepository)
        {
            _cacheService = cacheService;
            _promotionQueryRepository = promotionQueryRepository;
        }
        public async Task<PagedResult<GamePromotionResponse>> Handle(GetPagedPromotedCatalogGamesQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = CacheKeys.PagedPromotedCatalog(request.Page, request.PageSize, request.Genre, request.OnlyPromoted.GetValueOrDefault());

            var cachedGames = await _cacheService.GetAsync<PagedResult<GamePromotionResponse>>(cacheKey, cancellationToken);

            if(cachedGames != null)
            {
                return cachedGames;
            }

            var pagedCatalog = await _promotionQueryRepository.GetPagedCatalogByPromotionsAsync(request.Genre, request.Page, request.PageSize, cancellationToken);


            if (pagedCatalog.Items.Any())
            {
                await _cacheService.SetAsync(cacheKey, pagedCatalog, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return pagedCatalog;
        }
    }
}
