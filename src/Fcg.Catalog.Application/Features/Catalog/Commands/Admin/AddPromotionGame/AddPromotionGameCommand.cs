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

        public AddPromotionGameCommand(Guid gameId, decimal promotionValue, DateTime startDate, DateTime endDate)
        {
            GameId = gameId; PromotionValue = promotionValue; StartDate = startDate; EndDate = endDate;
        }
    }
}
