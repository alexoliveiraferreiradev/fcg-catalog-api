using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.EventHandlers
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
            await _cacheService.RemoveAsync("catalogo:todos");
            await _cacheService.RemoveAsync($"catalogo:jogo:detalhes:{notification.JogoId}");
            await _cacheService.RemoveAsync($"catalogo:promocao:detalhes:{notification.PromocaoId}");
            await _cacheService.RemoveByPrefixAsync("catalogo:pag:");
        }
    }
}
