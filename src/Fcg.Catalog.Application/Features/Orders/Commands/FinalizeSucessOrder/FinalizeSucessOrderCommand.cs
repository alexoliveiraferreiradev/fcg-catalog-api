using Fcg.Core.Abstractions.Enum;
using MediatR;

namespace Fcg.Catalog.Application.Features.Orders.Commands.FinalizeSucessOrder
{
    public record FinalizeSucessOrderCommand(Guid OrderId, Guid UserId, string NomeUsuario, string EmailUsuario, IEnumerable<Guid> JogosIds, PaymentStatus Status) : IRequest;
}
