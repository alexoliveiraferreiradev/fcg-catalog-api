using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Entities
{
    public class Order : AggregateRoot
    {
        public Guid UserId { get; private set; }
        public OrderStatus Status { get; private set; }
        public Price TotalAmount { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        private List<OrderGame> _games;
        public IReadOnlyCollection<OrderGame> Games => _games;

        protected Order()
        {
        }

        public Order(Guid userId)
        {
            this.UserId = userId;
            _games = new List<OrderGame>();
            Status = OrderStatus.Rascunho;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
            ValidateEntity();
        }

        protected override void ValidateEntity()
        {
            if (UserId == Guid.Empty) throw new DomainException(DomainMessages.OrderWithoutUser);
        }

        public void AddItem(Guid gameId, string gameName, decimal gameAmount)
        {
            if (Status != OrderStatus.Rascunho) throw new DomainException(DomainMessages.OrderGameNotDraft);
            if (gameId == Guid.Empty) throw new DomainException(DomainMessages.GameNotFound);
            if (_games.Any(j => j.GameId == gameId)) throw new DomainException(DomainMessages.OrderGameAlreadyAdded);

            _games.Add(new OrderGame(Id, gameId, gameName, gameAmount));
        }

        public void FinalizeOrder()
        {
            if (Status != OrderStatus.Rascunho) throw new DomainException(DomainMessages.OrderNotDraft);
            if (!_games.Any()) throw new DomainException(DomainMessages.OrderWithoutGames);
            Status = OrderStatus.Finalizado;
            CalculateTotalAmount();
            UpdatedAt = DateTime.UtcNow;
        }

        private void CalculateTotalAmount()
        {
            TotalAmount = new Price(_games.Sum(j => j.GameAmount));
        }
    }
}
