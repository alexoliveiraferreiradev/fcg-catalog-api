using Fcg.Catalogo.Application.Dtos.Promocao;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Jogos.Commands.AdicionarPromocaoJogo
{
    public class AdicionarPromocaoJogoCommand : IRequest<PromocaoResponse>
    {
        public Guid JogoId { get; set; }
        public decimal ValorPromocao { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }

        public AdicionarPromocaoJogoCommand()
        {
        }

        public AdicionarPromocaoJogoCommand(Guid jogoid, decimal valorPromocao, DateTime dataInicio, DateTime dataFim)
        {
            JogoId = jogoid; ValorPromocao = valorPromocao; DataInicio = dataInicio; DataFim = dataFim;
        }
    }
}
