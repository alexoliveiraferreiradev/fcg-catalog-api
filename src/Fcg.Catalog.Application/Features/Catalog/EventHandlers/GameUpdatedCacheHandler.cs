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
            _logger.LogInformation("[CatalogAPI] [Cache] Iniciando limpeza de cache apÃ³s a atualizaÃ§Ã£o do JogoId: {JogoId}", notification.GameId);
            await _cacheService.RemoveAsync("catalog:games");
            await _cacheService.RemoveAsync($"catalog:game:{notification.GameId}");
            await _cacheService.RemoveByPrefixAsync("catalog:pag:");
            _logger.LogInformation("[CatalogAPI] [Cache] Cache invalidado com sucesso para JogoId: {JogoId}", notification.GameId);
        }
    }
}
