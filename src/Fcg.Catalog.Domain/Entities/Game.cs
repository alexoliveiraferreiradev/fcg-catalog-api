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


        public Game(Name nomeJogo, Description descricaoJogo, Price precoJogo, GameGenre GameGenre)
        {
            Name = nomeJogo;
            Description = descricaoJogo;
            BasePrice = precoJogo;
            Genre = GameGenre;
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

        public void Update(Name novoNome, Description novaDescricao, Price novoPreco, GameGenre novoGenero)
        {
            if (!IsActive) throw new DomainException(DomainMessages.GameIsDeactivated);

            UpdateName(novoNome);
            UpdateDescription(novaDescricao);
            UpdatePrice(novoPreco);
            UpdateGenre(novoGenero);
            UpdatedAt = DateTime.UtcNow;
        }

        private void UpdateGenre(GameGenre novoGenero)
        {
            AssertionConcern.AssertArgumentRange((int)novoGenero, 1, 20, DomainMessages.GameGenreInvalid);
            if (Genre == novoGenero) return;
            Genre = novoGenero;
        }

        private void UpdatePrice(Price novoPreco)
        {
            if (BasePrice == novoPreco) return;
            BasePrice = novoPreco;
        }

        private void UpdateDescription(Description novaDescricao)
        {
            AssertionConcern.AssertArgumentNotNull(novaDescricao, DomainMessages.GameDescriptionRequired);
            if (Description == novaDescricao) return;
            Description = novaDescricao;
        }

        private void UpdateName(Name novoNome)
        {
            AssertionConcern.AssertArgumentNotNull(novoNome, DomainMessages.GameNameRequired);
            if (Name == novoNome) return;
            Name = novoNome;
        }

        public void AddPromotion(Price valorPromocao, Period EndDate)
        {
            if (valorPromocao.Amount >= BasePrice.Amount) throw new DomainException(DomainMessages.PromotionValueHigher);
            foreach (var p in _promotions.Where(x => x.IsActive)) p.Deactivate();
            _promotions.Add(new Promotion(Id, valorPromocao, EndDate));
            UpdatedAt = DateTime.UtcNow;
        }
        public void UpdatePromotion(Guid PromotionId, Price novoPreco, DateTime novaDataFim)
        {
            if (novoPreco.Amount >= BasePrice.Amount) throw new DomainException(DomainMessages.PromotionValueHigher);
            foreach (var p in _promotions.Where(x => x.Id == PromotionId)) p.UpdatePromotion(novoPreco, novaDataFim);
        }
        public Price GetCurrentPrice()
        {
            var promoAtiva = _promotions.FirstOrDefault(p => p.IsValid());

            var precoReferencia = promoAtiva != null ? promoAtiva.ValorPromocao : BasePrice;

            return new Price(precoReferencia.Amount);
        }
        public void DeactivatePromotion(Guid PromotionId)
        {
            var Promotion = _promotions.FirstOrDefault(x => x.Id == PromotionId);
            if (Promotion == null) throw new DomainException(DomainMessages.PromotionNotFound);
            Promotion.Deactivate();
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
