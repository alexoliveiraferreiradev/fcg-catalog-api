using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class GameReactivatedCacheHandler : INotificationHandler<GameDeactivatedEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<GameReactivatedCacheHandler> _logger;
        public GameReactivatedCacheHandler(ICacheService cacheService, ILogger<GameReactivatedCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task Handle(GameDeactivatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] [Cache] Iniciando limpeza de cache após a reativação do JogoId: {JogoId}", notification.GameId);
            await _cacheService.RemoveAsync("catalog:games");
            await _cacheService.RemoveAsync($"catalog:game:{notification.GameId}");
            await _cacheService.RemoveByPrefixAsync("catalog:pag:");
            _logger.LogInformation("[CatalogAPI] [Cache] Cache invalidado com sucesso para JogoId: {JogoId}", notification.GameId);
        }
    }
}
