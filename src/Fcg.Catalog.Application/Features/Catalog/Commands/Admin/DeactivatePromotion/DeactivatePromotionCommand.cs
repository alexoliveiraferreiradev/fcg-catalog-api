using MediatR;

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
        public Guid PromotionId { get; init; }
    }
}
