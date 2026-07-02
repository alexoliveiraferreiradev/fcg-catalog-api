using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Entities
{
    public class Promotion : EntityBase
    {
        public Promotion(Guid gameId, Price promotionPrice, Period period)
        {
            GameId = GameId;
            ValorPromocao = promotionPrice;
            IsActive = true;
            Period = period;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
            ValidateEntity();
        }
        protected Promotion() { }
        public Guid GameId { get; private set; }
        public Price ValorPromocao { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Period Period { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        protected override void ValidateEntity()
        {
            if (GameId == Guid.Empty) throw new DomainException(DomainMessages.GameNotFound);
            AssertionConcern.AssertArgumentNotNull(Period, DomainMessages.PeriodRequired);
        }

        public bool IsValid() =>
            IsActive && DateTime.UtcNow >= Period.StartDate && DateTime.UtcNow <= Period.EndDate;

        public void Deactivate()
        {
            if (!IsActive) throw new DomainException(DomainMessages.PromotionAlreadyDeactivated);
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdatePromotion(Price newPrice, DateTime newEndDate)
        {
            if (ValorPromocao == newPrice) return;
            ValorPromocao = newPrice;
            if (Period.EndDate != newEndDate)
            {
                Period = new Period(this.Period.StartDate, newEndDate);
            }
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
