using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class PromocaoAdicionadaCacheHandler : INotificationHandler<PromocaoAdicionadaEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<PromocaoAdicionadaCacheHandler> _logger;

        public PromocaoAdicionadaCacheHandler(ICacheService cacherService, ILogger<PromocaoAdicionadaCacheHandler> logger)
        {
            _cacheService = cacherService;
            _logger = logger;
        }

        public async Task Handle(PromocaoAdicionadaEvent notification, CancellationToken cancellationToken)
        {
            await _cacheService.RemoveAsync("Catalog:todos");
            await _cacheService.RemoveAsync($"Catalog:jogo:detalhes:{notification.JogoId}");
            await _cacheService.RemoveAsync($"Catalog:promocao:detalhes:{notification.PromocaoId}");
            await _cacheService.RemoveByPrefixAsync("Catalog:pag:");
        }
    }
}
