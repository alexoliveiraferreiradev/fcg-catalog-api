using Fcg.Catalog.Domain.Enum;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Entities
{
    public class Game : AggregateRoot
    {
        public Name Name { get; private set; }
        public Description Description { get; private set; }
        public Price BasePrice { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public GameGenre Genre { get; private set; }
        private List<Promotion> _promotions = new List<Promotion>();
        public IReadOnlyCollection<Promotion> Promotions => _promotions;

        protected Game()
        {
        }


        public Game(Name gameName, Description gameDescription, Price priceGame, GameGenre gameGenre)
        {
            Name = gameName;
            Description = gameDescription;
            BasePrice = priceGame;
            Genre = gameGenre;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
            ValidateEntity();
        }

        protected override void ValidateEntity()
        {
            AssertionConcern.AssertArgumentRange((int)Genre, 1, 20, DomainMessages.GameGenreInvalid);
        }

        public void Deactivate()
        {
            if (!IsActive) throw new DomainException(DomainMessages.GameIsDeactivated);
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reactivate()
        {
            if (IsActive) throw new DomainException(DomainMessages.GameAlreadyActive);
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(Name newName, Description nemGameDescription, Price newPrice, GameGenre newGenre)
        {
            if (!IsActive) throw new DomainException(DomainMessages.GameIsDeactivated);

            UpdateName(newName);
            UpdateDescription(nemGameDescription);
            UpdatePrice(newPrice);
            UpdateGenre(newGenre);
            UpdatedAt = DateTime.UtcNow;
        }

        private void UpdateGenre(GameGenre newGenre)
        {
            AssertionConcern.AssertArgumentRange((int)newGenre, 1, 20, DomainMessages.GameGenreInvalid);
            if (Genre == newGenre) return;
            Genre = newGenre;
        }

        private void UpdatePrice(Price newPrice)
        {
            if (BasePrice == newPrice) return;
            BasePrice = newPrice;
        }

        private void UpdateDescription(Description newGameDescription)
        {
            AssertionConcern.AssertArgumentNotNull(newGameDescription, DomainMessages.GameDescriptionRequired);
            if (Description == newGameDescription) return;
            Description = newGameDescription;
        }

        private void UpdateName(Name newName)
        {
            AssertionConcern.AssertArgumentNotNull(newName, DomainMessages.GameNameRequired);
            if (Name == newName) return;
            Name = newName;
        }

        public void AddPromotion(Price promotionValue, Period endDate)
        {
            if (promotionValue.Amount >= BasePrice.Amount) throw new DomainException(DomainMessages.PromotionValueHigher);
            foreach (var p in _promotions.Where(x => x.IsActive)) p.Deactivate();
            _promotions.Add(new Promotion(Id, promotionValue, endDate));
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdatePromotion(Guid promotionId, Price newPrice, DateTime newEndDate)
        {
            if (newPrice.Amount >= BasePrice.Amount) throw new DomainException(DomainMessages.PromotionValueHigher);
            foreach (var p in _promotions.Where(x => x.Id == promotionId)) p.UpdatePromotion(newPrice, newEndDate);
        }
        public Price GetCurrentPrice()
        {
            var promoAtiva = _promotions.FirstOrDefault(p => p.IsValid());

            var precoReferencia = promoAtiva != null ? promoAtiva.ValorPromocao : BasePrice;

            return new Price(precoReferencia.Amount);
        }
        public void DeactivatePromotion(Guid promotionId)
        {
            var promotion = _promotions.FirstOrDefault(x => x.Id == promotionId);
            if (promotion == null) throw new DomainException(DomainMessages.PromotionNotFound);
            promotion.Deactivate();
            UpdatedAt = DateTime.UtcNow;
        }

    }
}
