using MediatR;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public Guid PromotionId { get; init; }
        /// <summary>
        /// Novo preço promocional do game.
        /// </summary>
        [DefaultValue(0.00)]
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
        /// <param name="valorPromocao">Novo valor promocional.</param>
        /// <param name="EndDate">Nova data de término.</param>
        public UpdatePromotionCommand(decimal valorPromocao, DateTime EndDate)
        {
            NovoValorPromocao = valorPromocao; NovaDataFim = EndDate;
        }
    }
}
