using Fcg.Catalogo.Application.Features.Response;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.AdicionarPromocaoJogo
{
    public record AdicionarPromocaoJogoCommand : IRequest<PromocaoResponse>
    {
        public Guid JogoId { get; init; }
        public decimal ValorPromocao { get; init; }
        public DateTime DataInicio { get; init; }
        public DateTime DataFim { get; init; }

        public AdicionarPromocaoJogoCommand()
        {
        }

        public AdicionarPromocaoJogoCommand(Guid jogoid, decimal valorPromocao, DateTime dataInicio, DateTime dataFim)
        {
            JogoId = jogoid; ValorPromocao = valorPromocao; DataInicio = dataInicio; DataFim = dataFim;
        }
    }
}
