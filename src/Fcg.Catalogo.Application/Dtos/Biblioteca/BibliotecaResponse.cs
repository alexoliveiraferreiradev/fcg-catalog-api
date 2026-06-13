namespace Fcg.Catalogo.Application.Dtos.Biblioteca
{
    public class BibliotecaResponse
    {
        public Guid JogoId { get; set; }
        public string NomeJogo { get; set; }
        public string Descricao { get; set; }
        public string Genero { get; set; }
        public DateTime DataAquisicao { get; set; }

        public BibliotecaResponse()
        {
        }

        public BibliotecaResponse(Guid jogoid, string nomeJogo, string descricaojogo, string generoJogo, DateTime dataAquisicao)
        {
            JogoId = jogoid; NomeJogo = nomeJogo; Descricao = descricaojogo; Genero = generoJogo; DataAquisicao = dataAquisicao;
        }
    }

}
