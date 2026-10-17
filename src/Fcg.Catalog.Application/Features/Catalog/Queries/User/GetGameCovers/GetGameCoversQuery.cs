using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.User.GetGameCovers
{
    /// <summary>
    /// Capas de vários jogos de uma vez, inclusive desativados (a biblioteca mostra jogos que saíram do catálogo).
    /// </summary>
    public record GetGameCoversQuery(IReadOnlyCollection<Guid> Ids) : IRequest<IEnumerable<GameCoverResponse>>
    {
        public const int MaxIds = 50;
    }
}
