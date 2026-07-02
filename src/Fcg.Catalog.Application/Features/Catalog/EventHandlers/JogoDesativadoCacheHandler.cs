using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class JogoDesativadoCacheHandler : INotificationHandler<JogoDesativadoEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<JogoDesativadoCacheHandler> _logger;

        public JogoDesativadoCacheHandler(ICacheService cacheService, ILogger<JogoDesativadoCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task Handle(JogoDesativadoEvent notification, CancellationToken cancellationToken)
        {
            await _cacheService.RemoveAsync("Catalog:todos");
            await _cacheService.RemoveAsync($"Catalog:jogo:detalhes:{notification.JogoId}");
            await _cacheService.RemoveByPrefixAsync("Catalog:pag:");
        }
    }
}
