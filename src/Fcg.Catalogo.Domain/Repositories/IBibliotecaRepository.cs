using Fcg.Catalogo.Domain.Entities;

namespace Fcg.Catalogo.Domain.Repositories
{
    public interface IBibliotecaRepository
    {
        void Adicionar(Biblioteca biblioteca);
        void Atualizar(Biblioteca biblioteca);
        Task<Biblioteca?> ObterPorId(Guid id);
        Task<bool> VerificaSeUsuarioPossuiJogo(Guid usuarioId, Guid jogoId);
    }
}
