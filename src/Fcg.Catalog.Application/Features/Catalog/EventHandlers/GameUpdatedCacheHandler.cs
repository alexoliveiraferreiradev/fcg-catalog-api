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
            await _cacheService.RemoveAsync("Catalog:todos");
            await _cacheService.RemoveAsync($"Catalog:Game:detalhes:{notification.GameId}");
            await _cacheService.RemoveByPrefixAsync("Catalog:pag:");
        }
    }
}
