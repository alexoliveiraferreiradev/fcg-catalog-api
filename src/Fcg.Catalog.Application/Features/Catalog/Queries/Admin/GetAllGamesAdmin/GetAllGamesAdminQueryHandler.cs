using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Catalog.Queries.User.GetAllGames;
using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.Admin.GetAllGamesAdmin
{
    public class GetAllGamesAdminQueryHandler : IRequestHandler<GetAllGamesQuery, IEnumerable<GameResponse>>
    {
        private readonly IGameQueryRepository _gameQueryRepository;

        public GetAllGamesAdminQueryHandler(IGameQueryRepository gameQueryRepository)
        {
            _gameQueryRepository = gameQueryRepository;
        }

        public async Task<IEnumerable<GameResponse>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
        {
            return await _gameQueryRepository.GetAllGamesAsync(cancellationToken);
        }
    }
}
