using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.UpdatePromotion
{
    /// <summary>
    /// Comando para atualizar os dados de uma promoção.
    /// </summary>
    public record UpdatePromotionCommand : IRequest
    {
        /// <summary>
        /// Identificador único (GUID) da promoção a ser atualizada.
        /// </summary>
        public Guid PromotionId { get; init; }
        /// <summary>
        /// Identificador único (GUID) do game associado à promoção.
        /// </summary>
        public Guid GameId { get; init; }
        /// <summary>
        /// Novo preço promocional do game.
        /// </summary>
        public decimal NovoValorPromocao { get; init; }
        /// <summary>
        /// Nova data e hora de término da vigência da promoção.
        /// </summary>
        public DateTime NovaDataFim { get; init; }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public UpdatePromotionCommand()
        {
        }

        /// <summary>
        /// Construtor parametrizado.
        /// </summary>
        /// <param name="GameId">Identificador único do game.</param>
        /// <param name="valorPromocao">Novo valor promocional.</param>
        /// <param name="EndDate">Nova data de término.</param>
        public UpdatePromotionCommand(Guid GameId, decimal valorPromocao, DateTime EndDate)
        {
            this.GameId = GameId; NovoValorPromocao = valorPromocao; NovaDataFim = EndDate;
        }
    }
}
