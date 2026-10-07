using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.Admin.GetPromotionByGameId
{
    public class GetPromotionByGameIdQueryHandler : IRequestHandler<GetPromotionByGameIdQuery, PromotionResponse>
    {   
        private readonly IPromotionQueryRepository _promotionQueryRepository;

        public GetPromotionByGameIdQueryHandler(IPromotionQueryRepository promotionQueryRepository)
        {
            _promotionQueryRepository = promotionQueryRepository;
        }

        public async Task<PromotionResponse> Handle(GetPromotionByGameIdQuery request, CancellationToken cancellationToken)
        {        
            return await _promotionQueryRepository.GetPromotionByIdAsync(request.PromotionId, cancellationToken);
        }
    }
}

