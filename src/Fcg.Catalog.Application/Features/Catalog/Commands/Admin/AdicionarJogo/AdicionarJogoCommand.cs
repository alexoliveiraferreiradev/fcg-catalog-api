using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AdicionarJogo
{
    public record AdicionarJogoCommand : IRequest<JogoResponse>
    {
        public string Nome { get; init; }
        public string Descricao { get; init; }
        public decimal Preco { get; init; }
        public GeneroJogo Genero { get; init; }
    }
}
