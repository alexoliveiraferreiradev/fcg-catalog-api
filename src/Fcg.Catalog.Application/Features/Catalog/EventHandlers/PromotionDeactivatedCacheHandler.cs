using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class PromocaoDesativadaCacheHandler : INotificationHandler<PromotionDeactivatedEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<PromocaoDesativadaCacheHandler> _logger;
        public PromocaoDesativadaCacheHandler(ICacheService cacherService, ILogger<PromocaoDesativadaCacheHandler> logger)
        {
            _cacheService = cacherService;
            _logger = logger;
        }

        public async Task Handle(PromotionDeactivatedEvent notification, CancellationToken cancellationToken)
        {
            await _cacheService.RemoveAsync("Catalog:todos");
            await _cacheService.RemoveAsync($"Catalog:Promotion:detalhes:{notification.PromotionId}");
            await _cacheService.RemoveAsync($"Catalog:Game:detalhes:{notification.GameId}");
            await _cacheService.RemoveByPrefixAsync("Catalog:pag:");
        }
    }
}
