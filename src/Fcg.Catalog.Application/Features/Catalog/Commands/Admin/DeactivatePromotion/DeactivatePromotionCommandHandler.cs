using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivatePromotion
{
    public class DeactivatePromotionCommandHandler : IRequestHandler<DeactivatePromotionCommand>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeactivatePromotionCommandHandler> _logger;
        private readonly IMediator _mediator;
        public DeactivatePromotionCommandHandler(
            IGameRepository gameRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<DeactivatePromotionCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = gameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;   
        }

        public async Task Handle(DeactivatePromotionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para desativar promoção. PromocaoId: {PromocaoId}", request.PromotionId);

            var promotion = await _jogoRepository.GetPromotionById(request.PromotionId);
            if (promotion == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao desativar promoção. Promoção não encontrada. PromocaoId: {PromocaoId}", request.PromotionId);
                throw new DomainException(DomainMessages.PromotionNotFound);
            }

            var game = await _jogoRepository.GetById(promotion.GameId);
            if (game == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao desativar promoção. Jogo não encontrado. JogoId: {JogoId}", promotion.GameId);
                throw new DomainException(DomainMessages.GameNotFound);
            }

            game.DeactivatePromotion(request.PromotionId);
            _jogoRepository.Update(game);
            await _unitOfWork.CommitAsync();

            await _mediator.Publish(new PromotionDeactivatedEvent(game.Id,request.PromotionId), cancellationToken);

            _logger.LogInformation("[CatalogAPI] Promoção desativada com sucesso. PromocaoId: {PromocaoId}, JogoId: {JogoId}", request.PromotionId, promotion.GameId);
        }
    }
}
