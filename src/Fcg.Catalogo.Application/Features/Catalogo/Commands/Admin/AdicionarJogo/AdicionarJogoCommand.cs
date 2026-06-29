using Fcg.Catalogo.Application.Features.Response;
using Fcg.Catalogo.Domain.Enum;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.AdicionarJogo
{
    public record AdicionarJogoCommand : IRequest<JogoResponse>
    {
        public string Nome { get; init; }
        public string Descricao { get; init; }
        public decimal Preco { get; init; }
        public GeneroJogo Genero { get; init; }
    }
}
