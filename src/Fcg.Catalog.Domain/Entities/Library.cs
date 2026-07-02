using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Entities
{
    public class Library : AggregateRoot
    {       
        public Guid UserId { get; private set; }
        public virtual Game Game { get; private set; }
        public Guid GameId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public bool IsActive { get; private set; }

        public Library(Guid userId, Guid gameId)
        {
            UserId = userId;
            GameId = gameId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
            IsActive = true;
            ValidateEntity();
        }

        protected override void ValidateEntity()
        {
            AssertionConcern.AssertArgumentNotEquals(UserId, Guid.Empty, DomainMessages.UserNotFound);
            AssertionConcern.AssertArgumentNotEquals(GameId, Guid.Empty, DomainMessages.GameNotFound);
        }

    }
}
