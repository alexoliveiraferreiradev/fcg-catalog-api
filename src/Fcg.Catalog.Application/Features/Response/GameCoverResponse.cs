namespace Fcg.Catalog.Application.Features.Response
{
    public record GameCoverResponse
    {
        public Guid GameId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? CoverUrl { get; init; }
        /// <summary>false quando o jogo saiu da loja; a biblioteca continua mostrando, com o aviso "Fora da loja".</summary>
        public bool IsActive { get; init; }
    }
}
