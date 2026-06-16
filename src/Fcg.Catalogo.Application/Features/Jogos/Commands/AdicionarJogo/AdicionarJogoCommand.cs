using Fcg.Catalogo.Application.Dtos.Jogos;
using Fcg.Catalogo.Domain.Enum;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Catalogo.Application.Features.Jogos.Commands.AdicionarJogo
{
    public class AdicionarJogoCommand : IRequest<JogosResponse>
    {
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public GeneroJogo Genero { get; set; }

        public AdicionarJogoCommand()
        {
        }

        public AdicionarJogoCommand(string nomeJogo, string descricaoJogo, decimal precoJogo, GeneroJogo generoJogo)
        {
            Nome = nomeJogo; Descricao = descricaoJogo; Preco = precoJogo; Genero = generoJogo;
        }
    }
}
