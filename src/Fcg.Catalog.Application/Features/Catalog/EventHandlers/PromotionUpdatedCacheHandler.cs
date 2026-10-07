using Fcg.Catalog.Application.Common;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class PromotionUpdatedCacheHandler : INotificationHandler<PromotionUpdatedEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<PromotionUpdatedCacheHandler> _logger;   

        public PromotionUpdatedCacheHandler(ICacheService cacheService, ILogger<PromotionUpdatedCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task Handle(PromotionUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] [Cache] Iniciando limpeza de cache após a atualização de promoção. PromocaoId: {PromocaoId}, JogoId: {JogoId}", notification.PromotionId, notification.GameId);
            await _cacheService.RemoveAsync(CacheKeys.CatalogGames);
            await _cacheService.RemoveAsync($"{CacheKeys.CatalogGames}:{notification.GameId}");
            await _cacheService.RemoveAsync($"{CacheKeys.CatalogPromotion}:{notification.PromotionId}");
            await _cacheService.RemoveByPrefixAsync(CacheKeys.CatalogPages);
            await _cacheService.RemoveByPrefixAsync(CacheKeys.CatalogPromotionPages);
            _logger.LogInformation("[CatalogAPI] [Cache] Cache invalidado com sucesso para PromocaoId: {PromocaoId}", notification.PromotionId);
        }
    }
}
