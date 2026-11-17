using Fcg.Catalog.Domain.Resources;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Resources;

namespace Fcg.Catalog.Domain.Entities
{
    /// <summary>
    /// Nota de um usuário para um jogo (uma por par jogo/usuário).
    /// Guardar a nota individual permite trocar a média quando o usuário reavalia.
    /// </summary>
    public class GamesRating : EntityBase
    {
        public Guid GameId { get; private set; }
        public Guid UserId { get; private set; }
        public Avaliation Rating { get; private set; }
        public DateTime RatedAt { get; private set; }

        protected GamesRating()
        {
        }

        public GamesRating(Guid gameId, Guid userId, Avaliation rating, DateTime ratedAt)
        {
            GameId = gameId;
            UserId = userId;
            Rating = rating;
            RatedAt = ratedAt;
            ValidateEntity();
        }

        protected override void ValidateEntity()
        {
            AssertionConcern.AssertArgumentNotEmpty(GameId, DomainMessages.GameNotFound);
            AssertionConcern.AssertArgumentNotEmpty(UserId, CatalogMessages.RatingUserRequired);
            AssertionConcern.AssertArgumentNotNull(Rating, CatalogMessages.RatingOutOfRange);
        }

        /// <summary>Troca a nota; ignora eventos mais antigos que a nota atual (entrega fora de ordem).</summary>
        /// <returns>false quando o evento é antigo e a nota não mudou.</returns>
        public bool UpdateRating(Avaliation newRating, DateTime ratedAt)
        {
            if (ratedAt <= RatedAt) return false;

            AssertionConcern.AssertArgumentNotNull(newRating, CatalogMessages.RatingOutOfRange);
            Rating = newRating;
            RatedAt = ratedAt;
            return true;
        }
    }
}
