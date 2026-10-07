using Fcg.Catalog.Application.Common;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.User.GetGameById
{
    public class GetGameByIdQueryHandler : IRequestHandler<GetGameByIdQuery, GameResponse>
    {
        private readonly ICacheService _cacheService;
        private readonly IGameQueryRepository _gameQueryRepository;

        public GetGameByIdQueryHandler(ICacheService cacheService,
            IGameQueryRepository gameQueryRepository)
        {
            _cacheService = cacheService;
            _gameQueryRepository = gameQueryRepository;
        }

        public async Task<GameResponse> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"{CacheKeys.CatalogGames}:{request.GameId}";

            var cachedGame = await _cacheService.GetAsync<GameResponse>(cacheKey,cancellationToken);

            if(cachedGame != null)
            {
                return cachedGame;
            }

            var gameDetail = await _gameQueryRepository.GetGameByIdAsync(request.GameId, cancellationToken);

            if (gameDetail != null)
            {
                await _cacheService.SetAsync(cacheKey, gameDetail, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return gameDetail;
        }
    }
}
