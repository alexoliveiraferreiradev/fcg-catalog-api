using Fcg.Catalog.Application.Features.Orders.Commands.FinalizeFailedOrder;
using Fcg.Core.SharedContracts.MessageContracts;
using MassTransit;
using MediatR;

namespace Fcg.Catalog.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<IPaymentFailedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentFailedEventConsumer> _logger;

        public PaymentFailedEventConsumer(IMediator mediator, 
            ILogger<PaymentFailedEventConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IPaymentFailedEvent> context)
        {
            var order = context.Message;
            _logger.LogInformation("[CatalogAPI] PaymentFailedEvent recebido para o Pedido {OrderId}", order.OrderId);
            var finalizeCommand = new FinalizeFailedCommand(order.OrderId, order.Reason);
            await _mediator.Send(finalizeCommand);
        }
    }
}
