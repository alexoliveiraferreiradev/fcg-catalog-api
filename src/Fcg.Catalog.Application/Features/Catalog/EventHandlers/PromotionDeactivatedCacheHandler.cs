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
            await _cacheService.RemoveAsync("catalog:games");
            await _cacheService.RemoveAsync($"catalog:promotion:{notification.PromotionId}");
            await _cacheService.RemoveAsync($"catalog:game:{notification.GameId}");
            await _cacheService.RemoveByPrefixAsync("catalog:pag:");
            _logger.LogInformation("[CatalogAPI] [Cache] Cache invalidado com sucesso para PromocaoId: {PromocaoId}", notification.PromotionId);
        }
    }
}
