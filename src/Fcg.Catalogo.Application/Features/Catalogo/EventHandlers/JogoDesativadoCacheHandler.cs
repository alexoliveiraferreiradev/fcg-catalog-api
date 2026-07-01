using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.EventHandlers
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
            await _cacheService.RemoveAsync("catalogo:todos");
            await _cacheService.RemoveAsync($"catalogo:jogo:detalhes:{notification.JogoId}");
            await _cacheService.RemoveByPrefixAsync("catalogo:pag:");
        }
    }
}
