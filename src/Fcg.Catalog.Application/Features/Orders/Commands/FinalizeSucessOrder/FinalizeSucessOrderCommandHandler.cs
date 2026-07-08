using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Orders.Commands.FinalizeSucessOrder
{
    public class FinalizeSucessOrderCommandHandler : IRequestHandler<FinalizeSucessOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FinalizeSucessOrderCommandHandler> _logger;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;

        public FinalizeSucessOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork,
            ILogger<FinalizeSucessOrderCommandHandler> logger, ILibraryRepository libraryRepository, IMediator mediator, IPublishEndpoint publishEndpoint)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _libraryRepository = libraryRepository;
            _mediator = mediator;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(FinalizeSucessOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var orderUser = await _orderRepository.GetOrderById(request.OrderId);
                _logger.LogInformation("[CatalogAPI] Pagamento aprovado para o Pedido: {OrderId}. Adicionando Jogos à Biblioteca do Usuário: {UserId}", request.OrderId, request.UserId);

                foreach (var guidJogo in request.GameIds)
                {
                    var library = new UserLibrary(request.UserId, guidJogo);

                    _libraryRepository.Add(library);
                }
                await _mediator.Publish(new LibraryEvent(request.UserId));

                orderUser.FinalizeOrder();

                _logger.LogInformation("[CatalogAPI] Publicado LibraryEvent para o Usuário: {UserId}", request.UserId);


                await _unitOfWork.CommitAsync();
            }
            catch
            {
                await _publishEndpoint.Publish(new DeliveryFailedEvent(
                    request.OrderId,
                    request.UserId,
                    string.Empty,
                    string.Empty,
                    "Falha ao finalizar a Order e adicionar os Games à Library do Usuário."
                    ));
            }
        }
    }
}
