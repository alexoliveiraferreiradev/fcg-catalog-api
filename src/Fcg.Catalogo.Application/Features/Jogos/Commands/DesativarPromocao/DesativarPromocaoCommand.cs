using MediatR;

namespace Fcg.Catalogo.Application.Features.Jogos.Commands.DesativarPromocao
{
    public class DesativarPromocaoCommand : IRequest
    {
        public Guid PromocaoId { get; set; }
    }
}
