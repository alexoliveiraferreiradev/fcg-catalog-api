using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Enum;

namespace Fcg.Catalogo.Domain.Repositories
{
    public interface IJogoRepository
    {
        void Adicionar(Jogo jogo);
        void Atualizar(Jogo jogo);
        Task<bool> ExisteJogoComNome(string nomeJogo);
        Task<Jogo?> ObterPorId(Guid id);       
        Task<Promocao?> ObterPromocaoPorId(Guid id);        
        Task DesativaPromocoesInvalidas();
        Task<IEnumerable<Jogo>> ObterJogosPorIds(IEnumerable<Guid> jogosIds);
    }
}
