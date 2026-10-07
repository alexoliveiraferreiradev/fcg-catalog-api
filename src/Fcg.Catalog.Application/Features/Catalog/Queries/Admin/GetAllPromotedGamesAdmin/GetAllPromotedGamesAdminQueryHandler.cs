using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.Admin.GetAllPromotedGamesAdmin
{
    public class GetAllPromotedGamesAdminQueryHandler : IRequestHandler<GetAllPromotedGamesAdminQuery, IEnumerable<GamePromotionResponse>>
    {
        private readonly IPromotionQueryRepository _queryRepository;

        public GetAllPromotedGamesAdminQueryHandler(IPromotionQueryRepository queryRepository)
        {
            _queryRepository = queryRepository;
        }

        public async Task<IEnumerable<GamePromotionResponse>> Handle(GetAllPromotedGamesAdminQuery request, CancellationToken cancellationToken)
        {
            return await _queryRepository.GetAllPromotionGames();
        }
    }
}
