using Fcg.Catalog.Application.Features.Orders.Commands.FinalizeOrder;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Enum;
using Fcg.Core.Abstractions.Interfaces;
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
            _logger.LogInformation("[CatalogAPI] PaymentProcessedEvent recebido para Order {OrderId}", order.OrderId);
            var finalizeCommand = new FinalizeOrderCommand(order.OrderId, order.UserId, order.JogosIds, order.Status);
            await _mediator.Send(finalizeCommand);
        }
    }
}
