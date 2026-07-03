using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class LibraryCacheHandler : INotificationHandler<LibraryEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<LibraryCacheHandler> _logger;
        public LibraryCacheHandler(ICacheService cacheService, ILogger<LibraryCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;   
        }

        public async  Task Handle(LibraryEvent notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] [Cache] Iniciando invalidaÃ§Ã£o de cache da biblioteca do UsuarioId: {UsuarioId}", notification.UserId);
            await _cacheService.RemoveByPrefixAsync($"library:u_{notification.UserId}");
            _logger.LogInformation("[CatalogAPI] [Cache] Cache da biblioteca invalidado com sucesso para UsuarioId: {UsuarioId}", notification.UserId);
        }
    }
}
