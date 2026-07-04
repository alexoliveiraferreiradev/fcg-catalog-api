using MediatR;

namespace Fcg.Catalog.Application.Features.Orders.Commands.FinalizeFailedOrder
{
    public record FinalizeFailedCommand(Guid OrderId, string ReasonFailed) : IRequest;
}
