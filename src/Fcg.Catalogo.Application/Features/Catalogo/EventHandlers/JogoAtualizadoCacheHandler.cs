using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.EventHandlers
{
    public class JogoAtualizadoCacheHandler : INotificationHandler<JogoAtualizadoEvent>
    {
        private readonly ILogger<JogoAtualizadoCacheHandler> _logger;   
        private readonly ICacheService _cacheService;

        public JogoAtualizadoCacheHandler(ICacheService cacheService, ILogger<JogoAtualizadoCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }


        public async Task Handle(JogoAtualizadoEvent notification, CancellationToken cancellationToken)
        {
            await _cacheService.RemoveAsync("catalogo:todos");
            await _cacheService.RemoveAsync($"catalogo:jogo:detalhes:{notification.JogoId}");
            await _cacheService.RemoveByPrefixAsync("catalogo:pag:");
        }
    }
}
