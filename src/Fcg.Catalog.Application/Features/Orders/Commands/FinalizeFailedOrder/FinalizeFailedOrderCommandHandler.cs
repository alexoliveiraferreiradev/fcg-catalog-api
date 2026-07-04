using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Orders.Commands.FinalizeFailedOrder
{
    public class FinalizeFailedOrderCommandHandler : IRequestHandler<FinalizeFailedCommand>
    {
        private readonly ILogger<FinalizeFailedOrderCommandHandler> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public FinalizeFailedOrderCommandHandler(IOrderRepository orderRepository,
            IUnitOfWork unitOfWork, ILogger<FinalizeFailedOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }


        public async Task Handle(FinalizeFailedCommand request, CancellationToken cancellationToken)
        {
            var orderUser = await _orderRepository.GetOrderById(request.OrderId);

            orderUser.CancelOrder();

            _logger.LogInformation("[CatalogAPI] Pagamento não aprovado para o Pedido: {OrderId}", request.OrderId);

            _orderRepository.Update(orderUser);

            await _unitOfWork.CommitAsync();
        }
    }

}
