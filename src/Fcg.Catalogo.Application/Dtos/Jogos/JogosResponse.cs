using Fcg.Catalogo.Domain.Enum;

namespace Fcg.Catalogo.Application.Dtos.Jogos
{
    public class JogosResponse
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal PrecoOriginal { get; set; }
        public decimal PrecoAtual { get; set; }
        public GeneroJogo Genero { get; set; }
        public bool Ativo { get; set; }
        public bool TemDesconto => PrecoAtual < PrecoOriginal;

        public JogosResponse()
        {
        }

        public JogosResponse(Guid jogoId, string nomeJogo, string descricaoJogo, decimal precoOriginal, decimal precoAtual, GeneroJogo generoJogo)
        {
            Id = jogoId; Nome = nomeJogo; Descricao = descricaoJogo; PrecoOriginal = precoOriginal; PrecoAtual = precoAtual; Genero = generoJogo;
        }
    }
}
