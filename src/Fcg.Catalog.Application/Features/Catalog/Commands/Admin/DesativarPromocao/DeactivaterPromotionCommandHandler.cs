using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivatePromotion
{
    public class DesativarPromocaoCommandHandler : IRequestHandler<DesativarPromocaoCommand>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DesativarPromocaoCommandHandler> _logger;
        private readonly IMediator _mediator;
        public DesativarPromocaoCommandHandler(
            IGameRepository GameRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<DesativarPromocaoCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = GameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;   
        }

        public async Task Handle(DesativarPromocaoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para Deactivate promoção. PromotionId: {PromotionId}", request.PromotionId);

            var Promotion = await _jogoRepository.GetPromotionById(request.PromotionId);
            if (Promotion == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao Deactivate promoção. Promoção não encontrada. PromotionId: {PromotionId}", request.PromotionId);
                throw new DomainException(MensagensDominio.PromocaoNaoEncontrada);
            }

            var Game = await _jogoRepository.GetById(Promotion.GameId);
            if (Game == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao Deactivate promoção. Game não encontrado. GameId: {GameId}", Promotion.GameId);
                throw new DomainException(MensagensDominio.JogoNaoEncontrado);
            }

            Game.DeactivatePromotion(request.PromotionId);
            _jogoRepository.Update(Game);
            await _unitOfWork.CommitAsync();

            await _mediator.Publish(new PromotionDeactivatedEvent(Game.Id,request.PromotionId), cancellationToken);

            _logger.LogInformation("[CatalogAPI] Promoção desativada com sucesso. PromotionId: {PromotionId}, GameId: {GameId}", request.PromotionId, Promotion.GameId);
        }
    }
}
