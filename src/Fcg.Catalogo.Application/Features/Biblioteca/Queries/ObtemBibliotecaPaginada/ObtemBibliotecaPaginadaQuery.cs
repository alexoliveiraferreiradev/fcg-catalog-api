namespace Fcg.Catalogo.Application.Features.Biblioteca.Queries.ObtemBibliotecaPaginada
{
    public class ObtemBibliotecaPaginadaQuery
    {
        public Guid JogoId { get; set; }
        public string NomeJogo { get; set; }
        public string Descricao { get; set; }
        public string Genero { get; set; }
        public DateTime DataAquisicao { get; set; }

        public ObtemBibliotecaPaginadaQuery()
        {
        }

        public ObtemBibliotecaPaginadaQuery(Guid jogoid, string nomeJogo, string descricaojogo, string generoJogo, DateTime dataAquisicao)
        {
            JogoId = jogoid; NomeJogo = nomeJogo; Descricao = descricaojogo; Genero = generoJogo; DataAquisicao = dataAquisicao;
        }
    }
}
