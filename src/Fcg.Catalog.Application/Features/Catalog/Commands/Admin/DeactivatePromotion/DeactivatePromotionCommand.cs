using MediatR;
using System.ComponentModel;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivatePromotion
{
    /// <summary>
    /// Comando para desativar logicamente uma promoção existente.
    /// </summary>
    public record DeactivatePromotionCommand : IRequest
    {
        /// <summary>
        /// Identificador único (GUID) da promoção a ser desativada.
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid PromotionId { get; init; }
    }
}
