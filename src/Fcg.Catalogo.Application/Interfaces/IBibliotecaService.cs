using Fcg.Catalogo.Application.Common;
using Fcg.Catalogo.Application.Dtos.Biblioteca;

namespace Fcg.Catalogo.Application.Interfaces
{
    public interface IBibliotecaService
    {
        Task LiberarJogosAposPedido(Guid usuarioId, List<Guid> jogosIds);
        Task<PagedResult<BibliotecaResponse>> ObtemBibliotecaDoUsuarioPaginacao(Guid usuarioId, int pagina = 1, int tamanhoPagina = 10);
        Task<bool> VerificaSeUsuarioPossuiJogo(Guid usuarioId, Guid jogoId);
        Task<IEnumerable<Guid>> ObterIdsJogosDoUsuario(Guid usuarioId);
    }
}
