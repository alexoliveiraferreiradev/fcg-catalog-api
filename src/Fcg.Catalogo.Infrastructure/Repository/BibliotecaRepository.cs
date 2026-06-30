using Dapper;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Infrastructure.Persistence;
using Fcg.Core.Abstractions.Common;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Catalogo.Infrastructure.Repository
{
    public class BibliotecaRepository : IBibliotecaRepository
    {
        private readonly CatalogoDbContext _dbContext;

        public BibliotecaRepository(CatalogoDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Adicionar(Biblioteca biblioteca)
        {            
            _dbContext.Bibliotecas.Add(biblioteca);
        }

        public void Atualizar(Biblioteca biblioteca)
        {
            _dbContext.Update(biblioteca);
        }

        public async Task<IEnumerable<Guid>> ObterJogosAdquiridosPorUsuario(Guid usuarioId)
        {
            var connecetion = _dbContext.Database.GetDbConnection();
            const string sql = @"SELECT JogoId FROM Bibliotecas WHERE UsuarioId = @UsuarioId AND Ativo = 1";
            return await connecetion.QueryAsync<Guid>(sql, new { UsuarioId = usuarioId });
        }

        public async Task<Biblioteca?> ObterPorId(Guid id)
        {
           return await _dbContext.Bibliotecas.Where(x=>x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<bool> VerificaSeUsuarioPossuiJogo(Guid usuarioId, Guid jogoId)
        {
            return await _dbContext.Bibliotecas.AnyAsync(x => x.UsuarioId == usuarioId && x.JogoId == jogoId && x.Ativo);
        }


        

    }
}
