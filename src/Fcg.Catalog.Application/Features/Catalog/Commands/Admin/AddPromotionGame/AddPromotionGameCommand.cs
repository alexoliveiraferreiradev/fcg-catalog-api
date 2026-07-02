using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddPromotionGame
{
    public record AddPromotionGameCommand : IRequest<PromotionResponse>
    {
        public Guid GameId { get; init; }
        public decimal PromotionValue { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }

        public AddPromotionGameCommand()
        {
        }

        public AddPromotionGameCommand(Guid GameId, decimal promotionValue, DateTime StartDate, DateTime EndDate)
        {
            this.GameId = GameId; PromotionValue = promotionValue; this.StartDate = StartDate; this.EndDate = EndDate;
        }
    }
}
