namespace Fcg.Catalog.Application.Features.Biblioteca.Queries.ObtemBibliotecaPaginada
{
    public class BibliotecaItemResponse
    {
        public Guid JogoId { get; set; }
        public string NomeJogo { get; set; }
        public string Descricao { get; set; }
        public string Genero { get; set; }
        public DateTime DataAquisicao { get; set; }

        public BibliotecaItemResponse()
        {
        }

        public BibliotecaItemResponse(Guid jogoid, string nomeJogo, string descricaojogo, string generoJogo, DateTime dataAquisicao)
        {
            JogoId = jogoid; NomeJogo = nomeJogo; Descricao = descricaojogo; Genero = generoJogo; DataAquisicao = dataAquisicao;
        }
    }

}
