using Fcg.Catalog.Application.Features.Response;
using Fcg.Core.Abstractions.Common;

namespace Fcg.Catalog.Application.Common.Interfaces
{
    /// <summary>Motor de busca do catálogo, separado do banco relacional.</summary>
    public interface IGameSearchService
    {
        /// <summary>Copia o estado atual do jogo do banco para o índice (insere ou atualiza); remove do índice se o jogo não existir mais.</summary>
        Task IndexAsync(Guid gameId, CancellationToken cancellationToken);

        /// <summary>Recarrega todos os jogos do banco no índice.</summary>
        Task ReindexAllAsync(CancellationToken cancellationToken);

        /// <summary>Busca jogos ativos com tolerância a erro de digitação, do mais relevante para o menos relevante.</summary>
        Task<PagedResult<GameUserResponse>> SearchAsync(string term, int page, int pageSize, CancellationToken cancellationToken);
    }
}
