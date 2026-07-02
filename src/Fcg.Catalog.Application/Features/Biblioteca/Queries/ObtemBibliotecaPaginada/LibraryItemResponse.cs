namespace Fcg.Catalog.Application.Features.Library.Queries.ObtemBibliotecaPaginada
{
    public class BibliotecaItemResponse
    {
        public Guid GameId { get; set; }
        public string NomeJogo { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
        public DateTime DataAquisicao { get; set; }

        public BibliotecaItemResponse()
        {
        }

        public BibliotecaItemResponse(Guid GameId, string nomeJogo, string descricaojogo, string GameGenre, DateTime dataAquisicao)
        {
            GameId = GameId; NomeJogo = nomeJogo; Description = descricaojogo; Genre = GameGenre; DataAquisicao = dataAquisicao;
        }
    }

}
