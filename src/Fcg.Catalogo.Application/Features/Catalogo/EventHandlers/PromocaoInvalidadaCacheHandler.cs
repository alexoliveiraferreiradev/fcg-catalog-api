using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.EventHandlers
{
    public class PromocaoInvalidadaCacheHandler : INotificationHandler<PromocaoInvalidaEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<PromocaoInvalidadaCacheHandler> _logger;
        public PromocaoInvalidadaCacheHandler(ICacheService cacheService, ILogger<PromocaoInvalidadaCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task Handle(PromocaoInvalidaEvent notification, CancellationToken cancellationToken)
        {
            await _cacheService.RemoveAsync("catalogo:todos");
            await _cacheService.RemoveByPrefixAsync("catalogo:pag:");
            await _cacheService.RemoveByPrefixAsync("catalogo:jogo:detalhes:");
            await _cacheService.RemoveByPrefixAsync("catalogo:promocao:detalhes:");
        }
    }
}
