using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
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
            await _cacheService.RemoveAsync("Catalog:todos");
            await _cacheService.RemoveAsync($"Catalog:jogo:detalhes:{notification.JogoId}");
            await _cacheService.RemoveByPrefixAsync("Catalog:pag:");
        }
    }
}
