using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.EventHandlers
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
            await _cacheService.RemoveAsync("catalogo:todos");
            await _cacheService.RemoveAsync($"catalogo:jogo:detalhes:{notification.JogoId}");
            await _cacheService.RemoveByPrefixAsync("catalogo:pag:");
        }
    }
}
