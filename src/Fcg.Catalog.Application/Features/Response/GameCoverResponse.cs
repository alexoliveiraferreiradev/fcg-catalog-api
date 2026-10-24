namespace Fcg.Catalog.Application.Features.Response
{
    public record GameCoverResponse
    {
        public Guid GameId { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? CoverUrl { get; init; }
    }
}
