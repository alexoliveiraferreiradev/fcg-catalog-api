using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Catalogo.Application.Features.Jogos.Commands.AtualizarPromocao
{
    public class AtualizarPromocaoCommand : IRequest
    {
        public Guid PromocaoId { get; set; }
        public Guid JogoId { get; set; }
        public decimal NovoValorPromocao { get; set; }
        public DateTime NovaDataFim { get; set; }

        public AtualizarPromocaoCommand()
        {
        }

        public AtualizarPromocaoCommand(Guid jogoId, decimal valorPromocao, DateTime dataFim)
        {
            JogoId = jogoId; NovoValorPromocao = valorPromocao; NovaDataFim = dataFim;
        }
    }
}
