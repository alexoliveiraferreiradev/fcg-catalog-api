namespace Fcg.Catalogo.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task<T?> ObterAsync<T>(string chaveCache);
        Task DefinirAsync<T>(string chave, T valor, TimeSpan tempoExpiracao);
        Task RemoverAsync(string chaveCache);
        Task RemoverPorPrefixoAsync(string prefixo);
    }
}
