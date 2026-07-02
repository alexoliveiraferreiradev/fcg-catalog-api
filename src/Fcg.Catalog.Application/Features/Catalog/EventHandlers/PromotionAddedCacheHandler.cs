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
            await _cacheService.RemoveAsync("Catalog:todos");
            await _cacheService.RemoveAsync($"Catalog:Game:detalhes:{notification.GameId}");
            await _cacheService.RemoveAsync($"Catalog:Promotion:detalhes:{notification.PromotionId}");
            await _cacheService.RemoveByPrefixAsync("Catalog:pag:");
        }
    }
}
