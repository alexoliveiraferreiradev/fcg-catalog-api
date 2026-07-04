using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateInvalidPromotion
{
    public class DeactivatePromotionInvalidaCommandHandler : IRequestHandler<DeactivatePromotionInvalidaCommand>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeactivatePromotionInvalidaCommandHandler> _logger;
        private readonly IMediator _mediator;
        public DeactivatePromotionInvalidaCommandHandler(
            IGameRepository gameRepository,
            IUnitOfWork unitOfWork,
            ILogger<DeactivatePromotionInvalidaCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = gameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(DeactivatePromotionInvalidaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando desativação automática de promoções expiradas/inválidas.");

            await _jogoRepository.DeactivateInvalidPromotions();
            await _unitOfWork.CommitAsync();

            await _mediator.Publish(new InvalidPromotionEvent(), cancellationToken);

            _logger.LogInformation("[CatalogAPI] Desativação automática de promoções concluída.");
        }
    }
}
