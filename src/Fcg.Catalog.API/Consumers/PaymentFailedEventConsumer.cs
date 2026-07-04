using Fcg.Catalog.Application.Features.Orders.Commands.FinalizeFailedOrder;
using Fcg.Core.Abstractions.MessageContracts;
using MassTransit;
using MediatR;

namespace Fcg.Catalog.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentFailedEventConsumer> _logger;

        public PaymentFailedEventConsumer(IMediator mediator, 
            ILogger<PaymentFailedEventConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var order = context.Message;
            _logger.LogInformation("[CatalogAPI] PaymentFailedEvent recebido para o Pedido {OrderId}", order.OrderId);
            var finalizeCommand = new FinalizeFailedCommand(order.OrderId, order.Reason);
            await _mediator.Send(finalizeCommand);
        }
    }
}
