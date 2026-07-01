using Fcg.Catalogo.Application.Common.Interfaces;
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
        private readonly ICacheService _cacheService;
        
        public PaymentProcessedEventConsumer(IBibliotecaRepository bibliotecaRepository,
            IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _bibliotecaRepository = bibliotecaRepository;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
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

                await _cacheService.RemoveByPrefixAsync($"biblioteca:u_{pedido.UserId}");
            }
            
        }
    }
}
