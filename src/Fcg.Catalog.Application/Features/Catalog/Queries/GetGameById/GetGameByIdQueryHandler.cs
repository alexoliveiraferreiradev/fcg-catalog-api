using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.GetGameById
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
            var cacheKey = $"catalog:game:{request.GameId}";

            var cachedJogo = await _cacheService.GetAsync<GameResponse>(cacheKey,cancellationToken);

            if(cachedJogo != null)
            {
                return cachedJogo;
            }

            var jogoDetalhe = await _gameQueryRepository.GetGameByIdAsync(request.GameId, cancellationToken);

            if (jogoDetalhe != null)
            {
                await _cacheService.SetAsync(cacheKey, jogoDetalhe, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return jogoDetalhe;
        }
    }
}
