using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class JogoAdicionadoCacheHandler : INotificationHandler<JogoAdicionadoEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<JogoAdicionadoCacheHandler> _logger;

        public JogoAdicionadoCacheHandler(ILogger<JogoAdicionadoCacheHandler> logger, ICacheService cacheService)
        {
            _logger = logger;
            _cacheService = cacheService;
        }

        public async Task Handle(JogoAdicionadoEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[Cache] Iniciando limpeza de cache para o novo JogoId: {JogoId}", notification.JogoId);
            
            await _cacheService.RemoveAsync("Catalog:todos");
            await _cacheService.RemoveByPrefixAsync("Catalog:pag:");

            _logger.LogInformation("[Cache] Cache de listagem de jogos invalidado com sucesso.");
        }
    }
}
