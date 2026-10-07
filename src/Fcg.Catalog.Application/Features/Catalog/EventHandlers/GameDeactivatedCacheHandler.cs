using Fcg.Catalog.Application.Common;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class GameDeactivatedCacheHandler : INotificationHandler<GameDeactivatedEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<GameDeactivatedCacheHandler> _logger;

        public GameDeactivatedCacheHandler(ICacheService cacheService, ILogger<GameDeactivatedCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task Handle(GameDeactivatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] [Cache] Iniciando limpeza de cache após a desativação do JogoId: {JogoId}", notification.GameId);
            await _cacheService.RemoveAsync(CacheKeys.CatalogGames);
            await _cacheService.RemoveAsync($"{CacheKeys.CatalogGames}:{notification.GameId}");
            await _cacheService.RemoveByPrefixAsync(CacheKeys.CatalogPages);
            await _cacheService.RemoveByPrefixAsync(CacheKeys.CatalogPromotionPages);
            _logger.LogInformation("[CatalogAPI] [Cache] Cache invalidado com sucesso para JogoId: {JogoId}", notification.GameId);
        }
    }
}
