using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AdicionarPromocaoJogo
{
    public record AdicionarPromocaoJogoCommand : IRequest<PromocaoResponse>
    {
        public Guid GameId { get; init; }
        public decimal ValorPromocao { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }

        public AdicionarPromocaoJogoCommand()
        {
        }

        public AdicionarPromocaoJogoCommand(Guid GameId, decimal valorPromocao, DateTime StartDate, DateTime EndDate)
        {
            this.GameId = GameId; this.ValorPromocao = valorPromocao; this.StartDate = StartDate; this.EndDate = EndDate;
        }
    }
}
