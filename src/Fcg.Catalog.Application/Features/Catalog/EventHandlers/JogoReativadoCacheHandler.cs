using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class JogoReativadoCacheHandler : INotificationHandler<JogoDesativadoEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<JogoReativadoCacheHandler> _logger;
        public JogoReativadoCacheHandler(ICacheService cacheService, ILogger<JogoReativadoCacheHandler> logger)
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
