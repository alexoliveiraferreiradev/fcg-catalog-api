using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class PromotionUpdatedCacheHandler : INotificationHandler<PromotionUpdatedEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<PromotionUpdatedCacheHandler> _logger;   

        public PromotionUpdatedCacheHandler(ICacheService cacheService, ILogger<PromotionUpdatedCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task Handle(PromotionUpdatedEvent notification, CancellationToken cancellationToken)
        {
            await _cacheService.RemoveAsync("Catalog:todos");
            await _cacheService.RemoveAsync($"Catalog:Game:detalhes:{notification.GameId}");
            await _cacheService.RemoveAsync($"Catalog:Promotion:detalhes:{notification.PromotionId}");
            await _cacheService.RemoveByPrefixAsync("Catalog:pag:");
        }
    }
}
