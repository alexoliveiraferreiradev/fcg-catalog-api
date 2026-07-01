using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Events;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Core.Abstractions.Enum;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.API.Consumers
{
    public class PaymentProcessedEventConsumer : IConsumer<PaymentProcessedEvent>
    {
        private readonly IBibliotecaRepository _bibliotecaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ILogger<PaymentProcessedEventConsumer> _logger;
        
        public PaymentProcessedEventConsumer(IBibliotecaRepository bibliotecaRepository,
            IUnitOfWork unitOfWork, IMediator mediator, ILogger<PaymentProcessedEventConsumer> logger)
        {
            _bibliotecaRepository = bibliotecaRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
        {
            var pedido = context.Message;
            _logger.LogInformation("[CatalogoAPI] Recebido PaymentProcessedEvent para o Pedido: {OrderId}", pedido.OrderId);

            if(pedido.Status == PaymentStatus.Approved)
            {
                _logger.LogInformation("[CatalogoAPI] Pagamento aprovado para o Pedido: {OrderId}. Adicionando jogos à biblioteca do Usuário: {UserId}", pedido.OrderId, pedido.UserId);

                foreach(var guidJogo in pedido.JogosIds)
                {
                    var biblioteca = new Biblioteca(pedido.UserId, guidJogo);

                     _bibliotecaRepository.Adicionar(biblioteca);
                }
                
                await _unitOfWork.CommitAsync();

                await _mediator.Publish(new BibliotecaEvent(pedido.UserId));
                
                _logger.LogInformation("[CatalogoAPI] Publicado BibliotecaEvent para o Usuário: {UserId}", pedido.UserId);
            }
            else 
            {
                _logger.LogInformation("[CatalogoAPI] Pagamento não aprovado (Status: {Status}) para o Pedido: {OrderId}", pedido.Status, pedido.OrderId);
            }
        }
    }
}
