using Fcg.Catalogo.Application.Features.Response;
using Fcg.Catalogo.Domain.Enum;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.AtualizarJogo
{
    public record AtualizarJogoCommand : IRequest<JogoResponse>
    {
        public Guid JogoId { get; init; }
        public string NovoNome { get; init; }
        public string NovaDescricao { get; init; }
        public decimal NovoPreco { get; init; }
        public GeneroJogo NovoGenero { get; init; }

        public AtualizarJogoCommand()
        {
        }

        public AtualizarJogoCommand(string novoNome, string novaDescricao, decimal novoPreco, GeneroJogo novoGenero)
        {
            NovoNome = novoNome; NovaDescricao = novaDescricao; NovoPreco = novoPreco; NovoGenero = novoGenero;
        }
    }
}
