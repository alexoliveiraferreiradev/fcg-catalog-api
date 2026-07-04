using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Enum;
using Fcg.Core.Abstractions.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Orders.Commands.FinalizeOrder
{
    public class FinalizeOrderCommandHandler : IRequestHandler<FinalizeOrderCommand>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FinalizeOrderCommandHandler> _logger;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IMediator _mediator;

        public FinalizeOrderCommandHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork,
            ILogger<FinalizeOrderCommandHandler> logger, ILibraryRepository libraryRepository, IMediator mediator)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _libraryRepository = libraryRepository;
            _mediator = mediator;
        }

        public async Task Handle(FinalizeOrderCommand request, CancellationToken cancellationToken)
        {
            var orderUser = await _orderRepository.GetOrderById(request.OrderId);
            if (request.Status == PaymentStatus.Approved)
            {
                _logger.LogInformation("[CatalogAPI] Pagamento aprovado para o Order: {OrderId}. Adicionando Games à Library do Usuário: {UserId}", request.OrderId, request.UserId);

                foreach (var guidJogo in request.JogosIds)
                {
                    var library = new UserLibrary(request.UserId, guidJogo);

                    _libraryRepository.Add(library);
                }
                await _mediator.Publish(new LibraryEvent(request.UserId));               

                orderUser.FinalizeOrder();                

                _logger.LogInformation("[CatalogAPI] Publicado LibraryEvent para o Usuário: {UserId}", request.UserId);
            }
            else
            {
                orderUser.CancelOrder();    

                _logger.LogInformation("[CatalogAPI] Pagamento não aprovado (Status: {Status}) para o Order: {OrderId}", request.Status, request.OrderId);
            }
            _orderRepository.Update(orderUser);

            await _unitOfWork.CommitAsync();
        }
    }
}
