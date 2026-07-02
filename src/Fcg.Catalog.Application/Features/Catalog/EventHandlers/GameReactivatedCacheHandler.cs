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
            await _cacheService.RemoveAsync("Catalog:todos");
            await _cacheService.RemoveAsync($"Catalog:Game:detalhes:{notification.GameId}");
            await _cacheService.RemoveByPrefixAsync("Catalog:pag:");
        }
    }
}
