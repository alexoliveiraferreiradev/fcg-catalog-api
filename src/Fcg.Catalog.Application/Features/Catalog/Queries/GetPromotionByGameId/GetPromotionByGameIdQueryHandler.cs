using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetPromotionByGameId
{
    public class GetPromotionByGameIdQueryHandler : IRequestHandler<GetPromotionByGameIdQuery, PromotionResponse>
    {        
        private readonly ICacheService _cacheService;
        private readonly IPromotionQueryRepository _promotionQueryRepository;

        public GetPromotionByGameIdQueryHandler(IPromotionQueryRepository promotionQueryRepository,
            ICacheService cacheService)
        {
            _promotionQueryRepository = promotionQueryRepository;
            _cacheService = cacheService;
        }

        public async Task<PromotionResponse> Handle(GetPromotionByGameIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"catalog:promotion:{request.PromotionId}";

            var cachedPromocao = await _cacheService.GetAsync<PromotionResponse>(cacheKey, cancellationToken);

            if (cachedPromocao != null)
            {
                return cachedPromocao;
            }

            var promocaoDetalhe = await _promotionQueryRepository.GetPromotionByIdAsync(request.PromotionId, cancellationToken);

            if (promocaoDetalhe != null)
            {
                await _cacheService.SetAsync(cacheKey, promocaoDetalhe, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return promocaoDetalhe;
        }
    }
}

