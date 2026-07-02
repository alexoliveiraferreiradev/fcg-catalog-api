using Fcg.Catalog.Domain.Entities;

namespace Fcg.Catalog.Domain.Repositories
{
    public interface IBibliotecaRepository
    {
        void Adicionar(Biblioteca biblioteca);
        void Atualizar(Biblioteca biblioteca);
        Task<Biblioteca?> ObterPorId(Guid id);
        Task<bool> VerificaSeUsuarioPossuiJogo(Guid usuarioId, Guid jogoId);
        Task<IEnumerable<Guid>> ObterJogosAdquiridosPorUsuario(Guid usuarioId);

    }
}
