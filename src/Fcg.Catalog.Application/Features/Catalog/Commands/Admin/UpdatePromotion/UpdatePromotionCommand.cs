using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.UpdatePromotion
{
    public record UpdatePromotionCommand : IRequest
    {
        public Guid PromotionId { get; init; }
        public Guid GameId { get; init; }
        public decimal NovoValorPromocao { get; init; }
        public DateTime NovaDataFim { get; init; }

        public UpdatePromotionCommand()
        {
        }

        public UpdatePromotionCommand(Guid GameId, decimal valorPromocao, DateTime EndDate)
        {
            GameId = GameId; NovoValorPromocao = valorPromocao; NovaDataFim = EndDate;
        }
    }
}
