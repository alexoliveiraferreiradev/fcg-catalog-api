using MediatR;

namespace Fcg.Catalogo.Application.Features.Jogos.Commands.ReativarJogo
{
    public class ReativarJogoCommand : IRequest
    {
        public Guid JogoId { get; set; }
    }
}
