using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class PromocaoAtualizadaCacheHandler : INotificationHandler<PromocaoAtualizadaEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<PromocaoAtualizadaCacheHandler> _logger;   

        public PromocaoAtualizadaCacheHandler(ICacheService cacheService, ILogger<PromocaoAtualizadaCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task Handle(PromocaoAtualizadaEvent notification, CancellationToken cancellationToken)
        {
            await _cacheService.RemoveAsync("Catalog:todos");
            await _cacheService.RemoveAsync($"Catalog:jogo:detalhes:{notification.JogoId}");
            await _cacheService.RemoveAsync($"Catalog:promocao:detalhes:{notification.PromocaoId}");
            await _cacheService.RemoveByPrefixAsync("Catalog:pag:");
        }
    }
}
