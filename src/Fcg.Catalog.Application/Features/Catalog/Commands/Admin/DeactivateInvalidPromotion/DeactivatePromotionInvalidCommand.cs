using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateInvalidPromotion
{
    /// <summary>
    /// Comando interno para buscar e desativar automaticamente promoções expiradas ou inválidas no catálogo.
    /// </summary>
    public record DeactivatePromotionInvalidaCommand : IRequest
    {
    }
}
