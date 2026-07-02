using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.UpdatePromotion
{
    public record AtualizarPromocaoCommand : IRequest
    {
        public Guid PromotionId { get; init; }
        public Guid GameId { get; init; }
        public decimal NovoValorPromocao { get; init; }
        public DateTime NovaDataFim { get; init; }

        public AtualizarPromocaoCommand()
        {
        }

        public AtualizarPromocaoCommand(Guid GameId, decimal valorPromocao, DateTime EndDate)
        {
            GameId = GameId; NovoValorPromocao = valorPromocao; NovaDataFim = EndDate;
        }
    }
}
