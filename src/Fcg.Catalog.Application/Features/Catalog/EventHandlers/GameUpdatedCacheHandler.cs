using Fcg.Catalog.Application.Common;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class GameUpdatedCacheHandler : INotificationHandler<GameUpdatedEvent>
    {
        private readonly ILogger<GameUpdatedCacheHandler> _logger;   
        private readonly ICacheService _cacheService;

        public GameUpdatedCacheHandler(ICacheService cacheService, ILogger<GameUpdatedCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }


        public async Task Handle(GameUpdatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] [Cache] Iniciando limpeza de cache após a atualização do JogoId: {JogoId}", notification.GameId);
            await _cacheService.RemoveAsync(CacheKeys.CatalogGames);
            await _cacheService.RemoveAsync($"{CacheKeys.CatalogGames}:{notification.GameId}");
            await _cacheService.RemoveByPrefixAsync(CacheKeys.CatalogPages);
            await _cacheService.RemoveByPrefixAsync(CacheKeys.CatalogPromotionPages);
            _logger.LogInformation("[CatalogAPI] [Cache] Cache invalidado com sucesso para JogoId: {JogoId}", notification.GameId);
        }
    }
}
