using Fcg.Catalog.Application.Features.Orders.Commands.FinalizeSucessOrder;
using Fcg.Core.Abstractions.MessageContracts;
using MassTransit;
using MediatR;

namespace Fcg.Catalog.API.Consumers
{
    public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
    {        
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentProcessedEventConsumer> _logger;       

        public PaymentProcessedEventConsumer(IMediator mediator, ILogger<PaymentProcessedEventConsumer> logger)
        {
            _mediator = mediator;         
            _logger = logger;
         
        }

        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            var order = context.Message;
            _logger.LogInformation("[CatalogAPI] PaymentProcessedEvent recebido para o Pedido {OrderId}", order.OrderId);
            var finalizeCommand = new FinalizeSucessOrderCommand(order.OrderId, order.UserId, order.GameIds);
            await _mediator.Send(finalizeCommand);
        }
    }
}
