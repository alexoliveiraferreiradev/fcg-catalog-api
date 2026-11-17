using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.EventHandlers
{
    /// <summary>
    /// Mantém o índice de busca igual ao banco: toda mudança num jogo (dados, ativação, nota ou promoção)
    /// copia o estado atual dele para o índice.
    /// </summary>
    public class GameSearchIndexHandler :
        INotificationHandler<GameAddedEvent>,
        INotificationHandler<GameUpdatedEvent>,
        INotificationHandler<GameDeactivatedEvent>,
        INotificationHandler<GameReactivatedEvent>,
        INotificationHandler<PromotionAddedEvent>,
        INotificationHandler<PromotionUpdatedEvent>,
        INotificationHandler<PromotionDeactivatedEvent>
    {
        private readonly IGameSearchService _searchService;
        private readonly ILogger<GameSearchIndexHandler> _logger;

        public GameSearchIndexHandler(IGameSearchService searchService, ILogger<GameSearchIndexHandler> logger)
        {
            _searchService = searchService;
            _logger = logger;
        }

        public Task Handle(GameAddedEvent notification, CancellationToken cancellationToken) => IndexAsync(notification.GameId, cancellationToken);
        public Task Handle(GameUpdatedEvent notification, CancellationToken cancellationToken) => IndexAsync(notification.GameId, cancellationToken);
        public Task Handle(GameDeactivatedEvent notification, CancellationToken cancellationToken) => IndexAsync(notification.GameId, cancellationToken);
        public Task Handle(GameReactivatedEvent notification, CancellationToken cancellationToken) => IndexAsync(notification.GameId, cancellationToken);
        public Task Handle(PromotionAddedEvent notification, CancellationToken cancellationToken) => IndexAsync(notification.GameId, cancellationToken);
        public Task Handle(PromotionUpdatedEvent notification, CancellationToken cancellationToken) => IndexAsync(notification.GameId, cancellationToken);
        public Task Handle(PromotionDeactivatedEvent notification, CancellationToken cancellationToken) => IndexAsync(notification.GameId, cancellationToken);

        private async Task IndexAsync(Guid gameId, CancellationToken cancellationToken)
        {            
            try
            {
                await _searchService.IndexAsync(gameId, cancellationToken);
                _logger.LogInformation("[CatalogAPI] [Search] Índice atualizado para o JogoId: {JogoId}", gameId);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "[CatalogAPI] [Search] Falha ao atualizar o índice para o JogoId: {JogoId}", gameId);
            }
        }
    }
}
