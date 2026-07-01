using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.EventHandlers
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
            await _cacheService.RemoveAsync("catalogo:todos");
            await _cacheService.RemoveAsync($"catalogo:promocao:detalhes:{notification.PromocaoId}");
            await _cacheService.RemoveAsync($"catalogo:jogo:detalhes:{notification.JogoId}");
            await _cacheService.RemoveByPrefixAsync("catalogo:pag:");
        }
    }
}
