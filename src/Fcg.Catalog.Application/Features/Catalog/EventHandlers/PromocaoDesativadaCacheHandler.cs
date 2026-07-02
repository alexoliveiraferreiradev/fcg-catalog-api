using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class PromocaoDesativadaCacheHandler : INotificationHandler<PromocaoDesativadaEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<PromocaoDesativadaCacheHandler> _logger;
        public PromocaoDesativadaCacheHandler(ICacheService cacherService, ILogger<PromocaoDesativadaCacheHandler> logger)
        {
            _cacheService = cacherService;
            _logger = logger;
        }

        public async Task Handle(PromocaoDesativadaEvent notification, CancellationToken cancellationToken)
        {
            await _cacheService.RemoveAsync("Catalog:todos");
            await _cacheService.RemoveAsync($"Catalog:promocao:detalhes:{notification.PromocaoId}");
            await _cacheService.RemoveAsync($"Catalog:jogo:detalhes:{notification.JogoId}");
            await _cacheService.RemoveByPrefixAsync("Catalog:pag:");
        }
    }
}
