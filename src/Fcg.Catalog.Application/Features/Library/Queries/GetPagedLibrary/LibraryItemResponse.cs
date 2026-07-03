namespace Fcg.Catalog.Application.Features.Library.Queries.GetPagedLibrary
{
    public class BibliotecaItemResponse
    {
        public Guid GameId { get; set; }
        public string GameName { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public DateTime DataAquisicao { get; set; }

        public BibliotecaItemResponse()
        {
        }

        public BibliotecaItemResponse(Guid GameId, string GameName, string GameDescription, string GameGenre, DateTime dataAquisicao)
        {
            GameId = GameId; GameName = GameName; Description = GameDescription; Genre = GameGenre; DataAquisicao = dataAquisicao;
        }
    }

}
