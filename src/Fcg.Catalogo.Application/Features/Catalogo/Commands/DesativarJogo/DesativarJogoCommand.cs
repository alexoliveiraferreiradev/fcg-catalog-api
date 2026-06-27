using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.DesativarJogo
{
    public class DesativarJogoCommand : IRequest
    {
        public Guid JogoId { get; set; }
    }
}
