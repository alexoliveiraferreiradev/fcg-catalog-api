using Fcg.Catalog.Application.Common;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class PromotionDeactivatedCacheHandler : INotificationHandler<PromotionDeactivatedEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<PromotionDeactivatedCacheHandler> _logger;
        public PromotionDeactivatedCacheHandler(ICacheService cacherService, ILogger<PromotionDeactivatedCacheHandler> logger)
        {
            _cacheService = cacherService;
            _logger = logger;
        }

        public async Task Handle(PromotionDeactivatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] [Cache] Iniciando limpeza de cache após a desativação de promoção. PromocaoId: {PromocaoId}, JogoId: {JogoId}", notification.PromotionId, notification.GameId);
            await _cacheService.RemoveAsync(CacheKeys.CatalogGames);
            await _cacheService.RemoveAsync($"{CacheKeys.CatalogPromotion}:{notification.PromotionId}");
            await _cacheService.RemoveAsync($"{CacheKeys.CatalogGames}:{notification.GameId}");
            await _cacheService.RemoveByPrefixAsync(CacheKeys.CatalogPages);
            await _cacheService.RemoveByPrefixAsync(CacheKeys.CatalogPromotionPages);
            _logger.LogInformation("[CatalogAPI] [Cache] Cache invalidado com sucesso para PromocaoId: {PromocaoId}", notification.PromotionId);
        }
    }
}
