using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.AtualizarPromocao
{
    public record AtualizarPromocaoCommand : IRequest
    {
        public Guid PromocaoId { get; init; }
        public Guid JogoId { get; init; }
        public decimal NovoValorPromocao { get; init; }
        public DateTime NovaDataFim { get; init; }

        public AtualizarPromocaoCommand()
        {
        }

        public AtualizarPromocaoCommand(Guid jogoId, decimal valorPromocao, DateTime dataFim)
        {
            JogoId = jogoId; NovoValorPromocao = valorPromocao; NovaDataFim = dataFim;
        }
    }
}
