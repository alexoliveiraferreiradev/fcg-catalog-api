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
            IGameRepository GameRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<DeactivatePromotionCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = GameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;   
        }

        public async Task Handle(DeactivatePromotionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para desativar promoção. promoÃ§Ã£oId: {promoÃ§Ã£oId}", request.PromotionId);

            var Promotion = await _jogoRepository.GetPromotionById(request.PromotionId);
            if (Promotion == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao desativar promoção. Promoção não encontrada. promoÃ§Ã£oId: {promoÃ§Ã£oId}", request.PromotionId);
                throw new DomainException(DomainMessages.PromotionNotFound);
            }

            var Game = await _jogoRepository.GetById(Promotion.GameId);
            if (Game == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao desativar promoção. Jogo não encontrado. JogoId: {JogoId}", Promotion.GameId);
                throw new DomainException(DomainMessages.GameNotFound);
            }

            Game.DeactivatePromotion(request.PromotionId);
            _jogoRepository.Update(Game);
            await _unitOfWork.CommitAsync();

            await _mediator.Publish(new PromotionDeactivatedEvent(Game.Id,request.PromotionId), cancellationToken);

            _logger.LogInformation("[CatalogAPI] Promoção desativada com sucesso. promoÃ§Ã£oId: {promoÃ§Ã£oId}, JogoId: {JogoId}", request.PromotionId, Promotion.GameId);
        }
    }
}
