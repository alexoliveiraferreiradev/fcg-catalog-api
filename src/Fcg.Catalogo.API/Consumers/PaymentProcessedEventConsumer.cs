using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Core.Abstractions.Enum;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using MassTransit;

namespace Fcg.Catalogo.API.Consumers
{
    public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
    {
        private readonly IBibliotecaRepository _bibliotecaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;

        public PaymentProcessedEventConsumer(IBibliotecaRepository bibliotecaRepository,
            IUnitOfWork unitOfWork,IPublishEndpoint publishEndpoint)
        {
            _bibliotecaRepository = bibliotecaRepository;
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;
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

               await _publishEndpoint.Publish(new OrderApprovedEvent(
                    pedido.OrderId,
                    pedido.UserId,
                    pedido.NomeUsuario,
                    pedido.EmailUsuario,
                    pedido.CreatedAt
                    ));

                await _unitOfWork.CommitAsync();    
            }
            
        }
    }
}
