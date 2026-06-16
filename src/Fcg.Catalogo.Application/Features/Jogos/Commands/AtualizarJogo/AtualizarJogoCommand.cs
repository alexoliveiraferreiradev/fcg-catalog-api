using Fcg.Catalogo.Application.Dtos.Jogos;
using Fcg.Catalogo.Domain.Enum;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Catalogo.Application.Features.Jogos.Commands.AtualizarJogoCommand
{
    public class AtualizarJogoCommand : IRequest<JogosResponse>
    {
        public Guid JogoId { get; set; }
        public string NovoNome { get; set; }
        public string NovaDescricao { get; set; }
        public decimal NovoPreco { get; set; }
        public GeneroJogo NovoGenero { get; set; }

        public AtualizarJogoCommand()
        {
        }

        public AtualizarJogoCommand(string novoNome, string novaDescricao, decimal novoPreco, GeneroJogo novoGenero)
        {
            NovoNome = novoNome; NovaDescricao = novaDescricao; NovoPreco = novoPreco; NovoGenero = novoGenero;
        }
    }
}
