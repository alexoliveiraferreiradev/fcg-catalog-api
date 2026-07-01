namespace Fcg.Catalogo.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string chaveCache,CancellationToken cancellation);
        Task SetAsync<T>(string chave, T valor, TimeSpan tempoExpiracao, CancellationToken cancellation);
        Task RemoveAsync(string chaveCache);
        Task RemoveByPrefixAsync(string prefixo);
    }
}
