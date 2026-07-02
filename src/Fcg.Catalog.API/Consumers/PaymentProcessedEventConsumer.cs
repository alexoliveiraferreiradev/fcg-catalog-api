using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Enum;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.API.Consumers
{
    public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
    {
        private readonly ILibraryRepository _bibliotecaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentProcessedEventConsumer> _logger;
        
        public PaymentProcessedEventConsumer(ILibraryRepository LibraryRepository,
            IUnitOfWork unitOfWork, IMediator mediator, ILogger<PaymentProcessedEventConsumer> logger)
        {
            _bibliotecaRepository = LibraryRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            var Order = context.Message;
            _logger.LogInformation("[CatalogAPI] Recebido PaymentProcessedEvent para o Order: {OrderId}", Order.OrderId);

            if(Order.Status == PaymentStatus.Approved)
            {
                _logger.LogInformation("[CatalogAPI] Pagamento Approved para o Order: {OrderId}. Adicionando Games à Library do Usuário: {UserId}", Order.OrderId, Order.UserId);

                foreach(var guidJogo in Order.JogosIds)
                {
                    var Library = new Library(Order.UserId, guidJogo);

                     _bibliotecaRepository.Add(Library);
                }
                
                await _unitOfWork.CommitAsync();

                await _mediator.Publish(new LibraryEvent(Order.UserId));
                
                _logger.LogInformation("[CatalogAPI] Publicado LibraryEvent para o Usuário: {UserId}", Order.UserId);
            }
            else 
            {
                _logger.LogInformation("[CatalogAPI] Pagamento não Approved (Status: {Status}) para o Order: {OrderId}", Order.Status, Order.OrderId);
            }
        }
    }
}
