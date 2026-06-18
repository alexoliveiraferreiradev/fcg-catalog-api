using Fcg.Catalogo.Domain.Entities;

namespace Fcg.Catalogo.Domain.Repositories
{
    public interface IBibliotecaRepository
    {
        Task Adicionar(Biblioteca biblioteca);
        Task Atualizar(Biblioteca biblioteca);
        Task<Biblioteca?> ObterPorId(Guid id);
        Task<bool> VerificaSeUsuarioPossuiJogo(Guid usuarioId, Guid jogoId);
        Task SaveChanges();
    }
}
