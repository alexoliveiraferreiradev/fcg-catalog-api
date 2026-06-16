using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Enum;

namespace Fcg.Catalogo.Domain.Repositories
{
    public interface IJogoRepository
    {
        Task Adicionar(Jogo jogo);
        Task Atualizar(Jogo jogo);
        Task<Jogo> ObterPorId(Guid id);        
        Task SaveChanges();                
        Task<Promocao?> ObterPromocaoPorId(Guid id);        
        Task DesativaPromocoesInvalidas();
    }
}
