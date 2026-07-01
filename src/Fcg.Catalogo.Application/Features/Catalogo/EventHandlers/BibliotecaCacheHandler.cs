using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.EventHandlers
{
    public class BibliotecaCacheHandler : INotificationHandler<BibliotecaEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<BibliotecaCacheHandler> _logger;
        public BibliotecaCacheHandler(ICacheService cacheService, ILogger<BibliotecaCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;   
        }

        public async  Task Handle(BibliotecaEvent notification, CancellationToken cancellationToken)
        {
            await _cacheService.RemoveByPrefixAsync($"biblioteca:u_{notification.UsuarioId}");
        }
    }
}
