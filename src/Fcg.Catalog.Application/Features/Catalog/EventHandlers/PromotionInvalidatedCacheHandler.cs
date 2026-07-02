using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class PromotionInvalidatedCacheHandler : INotificationHandler<InvalidPromotionEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<PromotionInvalidatedCacheHandler> _logger;
        public PromotionInvalidatedCacheHandler(ICacheService cacheService, ILogger<PromotionInvalidatedCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task Handle(InvalidPromotionEvent notification, CancellationToken cancellationToken)
        {
            await _cacheService.RemoveAsync("Catalog:todos");
            await _cacheService.RemoveByPrefixAsync("Catalog:pag:");
            await _cacheService.RemoveByPrefixAsync("Catalog:Game:detalhes:");
            await _cacheService.RemoveByPrefixAsync("Catalog:Promotion:detalhes:");
        }
    }
}
