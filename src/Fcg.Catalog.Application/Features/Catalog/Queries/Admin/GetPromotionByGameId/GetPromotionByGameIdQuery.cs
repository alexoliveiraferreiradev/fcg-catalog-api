using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.Admin.GetPromotionByGameId
{
    public record GetPromotionByGameIdQuery(Guid PromotionId) :IRequest<PromotionResponse>;
}
