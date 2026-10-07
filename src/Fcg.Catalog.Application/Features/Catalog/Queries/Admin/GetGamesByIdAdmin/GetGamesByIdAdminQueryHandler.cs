using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Catalog.Queries.User.GetGameById;
using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.Admin.GetGamesByIdAdmin
{
    public class GetGamesByIdAdminQueryHandler : IRequestHandler<GetGameByIdQuery, GameResponse>
    {        
        private readonly IGameQueryRepository _gameQueryRepository;

        public GetGamesByIdAdminQueryHandler(IGameQueryRepository gameQueryRepository)
        {     
            _gameQueryRepository = gameQueryRepository;
        }

        public async Task<GameResponse> Handle(GetGameByIdQuery request, CancellationToken cancellationToken)
        {           
            return await _gameQueryRepository.GetGameByIdAsync(request.GameId, cancellationToken);
        }
    }
}
