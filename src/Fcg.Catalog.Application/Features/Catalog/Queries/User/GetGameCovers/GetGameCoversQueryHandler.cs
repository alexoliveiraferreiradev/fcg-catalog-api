using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.User.GetGameCovers
{
    public class GetGameCoversQueryHandler : IRequestHandler<GetGameCoversQuery, IEnumerable<GameCoverResponse>>
    {
        private readonly IGameQueryRepository _gameQueryRepository;

        public GetGameCoversQueryHandler(IGameQueryRepository gameQueryRepository)
        {
            _gameQueryRepository = gameQueryRepository;
        }

        // Sem cache: a capa editada pelo admin aparece na hora, e a consulta é por chave primária.
        public async Task<IEnumerable<GameCoverResponse>> Handle(GetGameCoversQuery request, CancellationToken cancellationToken)
        {
            var ids = request.Ids.Distinct().ToList();
            return await _gameQueryRepository.GetGameCoversAsync(ids, cancellationToken);
        }
    }
}
