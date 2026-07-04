using Fcg.Catalog.Application.Features.Response;
using MediatR;
using System.ComponentModel;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddPromotionGame
{
    /// <summary>
    /// Comando para adicionar uma nova promoção a um game.
    /// </summary>
    public record AddPromotionGameCommand : IRequest<PromotionResponse>
    {
        /// <summary>
        /// Identificador único (GUID) do game ao qual a promoção será associada.
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid GameId { get; init; }
        /// <summary>
        /// Preço promocional do game (deve ser menor que o preço original).
        /// </summary>
        [DefaultValue(29.99)]
        public decimal PromotionValue { get; init; }
        /// <summary>
        /// Data e hora de início da vigência da promoção.
        /// </summary>
        [DefaultValue("2026-07-04T17:40:00Z")]
        public DateTime StartDate { get; init; }
        /// <summary>
        /// Data e hora de término da vigência da promoção.
        /// </summary>
        [DefaultValue("2026-07-14T17:40:00Z")]
        public DateTime EndDate { get; init; }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public AddPromotionGameCommand()
        {
        }

        /// <summary>
        /// Construtor parametrizado.
        /// </summary>
        /// <param name="gameId">Identificador único do game.</param>
        /// <param name="promotionValue">Preço promocional.</param>
        /// <param name="startDate">Data de início.</param>
        /// <param name="endDate">Data de término.</param>
        public AddPromotionGameCommand(Guid gameId, decimal promotionValue, DateTime startDate, DateTime endDate)
        {
            GameId = gameId; PromotionValue = promotionValue; StartDate = startDate; EndDate = endDate;
        }
    }
}
