using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetAllGames
{
    public class GetAllGamesQueryHandler : IRequestHandler<GetAllGamesQuery, IEnumerable<GameResponse>>
    {        
        private readonly IGameQueryRepository _gameQueryRepository;
        private readonly ICacheService _cacheService;

        public GetAllGamesQueryHandler(ICacheService cacheService, IGameQueryRepository gameQueryRepository)
        {            
            _cacheService = cacheService;
            _gameQueryRepository = gameQueryRepository;
        }

        public async Task<IEnumerable<GameResponse>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "catalog:games";

            var cachedCatalog = await _cacheService.GetAsync<IEnumerable<GameResponse>>(cacheKey,cancellationToken);

            if (cachedCatalog != null) {
                return cachedCatalog;
            }

            var pagedCatalog = await _gameQueryRepository.GetAllGamesAsync(cancellationToken);

            if (pagedCatalog != null)
            {
                await _cacheService.SetAsync(cacheKey, pagedCatalog, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return pagedCatalog;
        }
    }
}
