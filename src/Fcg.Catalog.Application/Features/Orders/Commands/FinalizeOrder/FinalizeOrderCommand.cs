using Fcg.Core.Abstractions.Enum;
using MediatR;

namespace Fcg.Catalog.Application.Features.Orders.Commands.FinalizeOrder
{
    public record FinalizeOrderCommand(Guid OrderId, Guid UserId, IEnumerable<Guid> JogosIds, PaymentStatus Status) : IRequest;
}
