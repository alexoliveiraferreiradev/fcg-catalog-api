using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class GameAddedCacheHandler : INotificationHandler<GameAddedEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<GameAddedCacheHandler> _logger;

        public GameAddedCacheHandler(ILogger<GameAddedCacheHandler> logger, ICacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task Handle(GameAddedEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Cache] Iniciando limpeza de cache para o novo GameId: {GameId}", notification.GameId);
            
            await _cacheService.RemoveAsync("Catalog:todos");
            await _cacheService.RemoveByPrefixAsync("Catalog:pag:");

            _logger.LogInformation("[Cache] Cache de listagem de Games invalidado com sucesso.");
        }
    }
}
