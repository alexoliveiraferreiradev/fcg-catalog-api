using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Events;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Core.Abstractions.Enum;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using MassTransit;
using MediatR;

namespace Fcg.Catalogo.API.Consumers
{
    public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
    {
        private readonly IBibliotecaRepository _bibliotecaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        
        public PaymentProcessedEventConsumer(IBibliotecaRepository bibliotecaRepository,
            IUnitOfWork unitOfWork, IMediator mediator)
        {
            _bibliotecaRepository = bibliotecaRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
        }

        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            var pedido = context.Message;
            if(pedido.Status == PaymentStatus.Approved)
            {
                foreach(var guidJogo in pedido.JogosIds)
                {
                    var biblioteca = new Biblioteca(pedido.UserId, guidJogo);

                     _bibliotecaRepository.Adicionar(biblioteca);
                }

               await context.Publish(new OrderApprovedEvent(
                    pedido.OrderId,
                    pedido.UserId,
                    pedido.EmailUsuario,
                    pedido.NomeUsuario,
                    pedido.CreatedAt
                    ));

                await _unitOfWork.CommitAsync();

                await _mediator.Publish(new BibliotecaEvent(pedido.UserId));
                
            }
            
        }
    }
}
