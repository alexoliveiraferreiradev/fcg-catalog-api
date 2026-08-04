using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.SharedContracts.MessageContracts;

namespace Fcg.Catalog.Application.IntegrationEvent
{
    public class RepublishGamesEvent
    {
        private readonly IGameRepository _gameRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IIntegrationEventPublisher _integrationEventPublisher;

        public RepublishGamesEvent(IGameRepository gameRepository, IUnitOfWork unitOfWork,
            IIntegrationEventPublisher integrationEventPublisher)
        {
            _gameRepository = gameRepository;
            _unitOfWork = unitOfWork;
            _integrationEventPublisher = integrationEventPublisher;
        }

        public async Task Handle()
        {
            var games = await _gameRepository.GetAllGamesAsync();
            var now = DateTime.UtcNow;
            foreach (var game in games)
            {
                var gamePrice = game.Promotions.Any() ? game.Promotions.First().ValorPromocao.Amount : game.BasePrice.Amount;
                await _integrationEventPublisher.PublishAsync<IGameCreatedIntegrationEvent>(new
                {
                    GameId = game.Id,
                    Name = game.Name.Value,
                    Price = gamePrice,
                    IsAvaiable = game.IsActive,
                    Description = game.Description.Value,
                    Genre = game.Genre.ToString(),
                    OccurredAt = now
                });

            }
            await _unitOfWork.CommitAsync();
        }
    }
}
