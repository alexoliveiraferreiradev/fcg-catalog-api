using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Entities
{
    public class Promotion : EntityBase
    {
        public Promotion(Guid GameId, Price valorPromocao, Period Period)
        {
            this.GameId = GameId;
            ValorPromocao = valorPromocao;
            IsActive = true;
            this.Period = Period;
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

        public void UpdatePromotion(Price novoPreco, DateTime novaDataFim)
        {
            if (ValorPromocao == novoPreco) return;
            ValorPromocao = novoPreco;
            if (Period.EndDate != novaDataFim)
            {
                Period = new Period(this.Period.StartDate, novaDataFim);
            }
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
