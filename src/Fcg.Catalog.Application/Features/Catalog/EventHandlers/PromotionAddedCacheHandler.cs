using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class PromotionAddedCacheHandler : INotificationHandler<PromotionAddedEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<PromotionAddedCacheHandler> _logger;

        public PromotionAddedCacheHandler(ICacheService cacherService, ILogger<PromotionAddedCacheHandler> logger)
        {
            _cacheService = cacherService;
            _logger = logger;
        }

        public async Task Handle(PromotionAddedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] [Cache] Iniciando limpeza de cache apÃ³s a adiÃ§Ã£o de promoÃ§Ã£o. promoÃ§Ã£oId: {promoÃ§Ã£oId}, JogoId: {JogoId}", notification.PromotionId, notification.GameId);
            await _cacheService.RemoveAsync("catalog:games");
            await _cacheService.RemoveAsync($"catalog:game:{notification.GameId}");
            await _cacheService.RemoveAsync($"catalog:promotion:{notification.PromotionId}");
            await _cacheService.RemoveByPrefixAsync("catalog:pag:");
            _logger.LogInformation("[CatalogAPI] [Cache] Cache invalidado com sucesso para promoÃ§Ã£oId: {promoÃ§Ã£oId}", notification.PromotionId);
        }
    }
}
