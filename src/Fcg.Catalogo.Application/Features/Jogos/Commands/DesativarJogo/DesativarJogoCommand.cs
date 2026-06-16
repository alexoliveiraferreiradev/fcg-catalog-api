using MediatR;

namespace Fcg.Catalogo.Application.Features.Jogos.Commands.DesativarJogo
{
    public class DesativarJogoCommand : IRequest
    {
        public Guid JogoId { get; set; }
    }
}
