using Fcg.Catalog.Application.Common;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class PromotionInvalidatedCacheHandler : INotificationHandler<InvalidPromotionEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<PromotionInvalidatedCacheHandler> _logger;
        public PromotionInvalidatedCacheHandler(ICacheService cacheService, ILogger<PromotionInvalidatedCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task Handle(InvalidPromotionEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] [Cache] Iniciando invalidação em massa de cache de promoções expiradas.");
            await _cacheService.RemoveAsync(CacheKeys.CatalogGames);
            await _cacheService.RemoveByPrefixAsync(CacheKeys.CatalogPages);
            await _cacheService.RemoveByPrefixAsync($"{CacheKeys.CatalogGames}:");
            await _cacheService.RemoveByPrefixAsync($"{CacheKeys.CatalogPromotion}:");
            await _cacheService.RemoveByPrefixAsync(CacheKeys.CatalogPromotionPages);
            _logger.LogInformation("[CatalogAPI] [Cache] Cache de promoções invalidado em massa com sucesso.");
        }
    }
}
