using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.DesativarPromocao
{
    public class DesativarPromocaoCommand : IRequest
    {
        public Guid PromocaoId { get; set; }
    }
}
