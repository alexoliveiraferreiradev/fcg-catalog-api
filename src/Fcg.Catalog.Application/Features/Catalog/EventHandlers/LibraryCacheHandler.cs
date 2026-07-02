using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    public class BibliotecaCacheHandler : INotificationHandler<LibraryEvent>
    {
        private readonly ICacheService _cacheService;
        private readonly ILogger<BibliotecaCacheHandler> _logger;
        public BibliotecaCacheHandler(ICacheService cacheService, ILogger<BibliotecaCacheHandler> logger)
        {
            _cacheService = cacheService;
            _logger = logger;   
        }

        public async  Task Handle(LibraryEvent notification, CancellationToken cancellationToken)
        {
            await _cacheService.RemoveByPrefixAsync($"Library:u_{notification.UserId}");
        }
    }
}
