using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AtualizarJogo
{
    public record UpdateGameCommand : IRequest<JogoResponse>
    {
        public Guid GameId { get; init; }
        public string NovoNome { get; init; }
        public string NovaDescricao { get; init; }
        public decimal NovoPreco { get; init; }
        public GameGenre NovoGenero { get; init; }

        public UpdateGameCommand()
        {
        }

        public UpdateGameCommand(string novoNome, string novaDescricao, decimal novoPreco, GameGenre novoGenero)
        {
            NovoNome = novoNome; NovaDescricao = novaDescricao; NovoPreco = novoPreco; NovoGenero = novoGenero;
        }
    }
}
