using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.UpdatePromotion
{
    public class UpdatePromotionCommandHandler : IRequestHandler<UpdatePromotionCommand>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdatePromotionCommandHandler> _logger;
        private readonly IMediator _mediator;

        public UpdatePromotionCommandHandler(
            IGameRepository gameRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdatePromotionCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = gameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(UpdatePromotionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para atualizar promoção. PromocaoId: {PromocaoId}, NovoValor: {NovoValor}", request.PromotionId, request.NovoValorPromocao);

            var promotion = await _jogoRepository.GetPromotionById(request.PromotionId);
            if (promotion == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao atualizar promoção. Promoção não encontrada. PromocaoId: {PromocaoId}", request.PromotionId);
                throw new DomainException(DomainMessages.PromotionNotFound);
            }

            var game = await _jogoRepository.GetById(promotion.GameId);
            if (game == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao atualizar promoção. Jogo não encontrado. JogoId: {JogoId}", promotion.GameId);
                throw new DomainException(DomainMessages.GameNotFound);
            }

            var novoPreco = new Price(request.NovoValorPromocao);

            game.UpdatePromotion(request.PromotionId, novoPreco, request.NovaDataFim);
            _jogoRepository.Update(game);
            await _unitOfWork.CommitAsync();

            var novaPromocao = game.Promotions.First();

            await _mediator.Publish(new PromotionUpdatedEvent(game.Id, novaPromocao.Id), cancellationToken);

            _logger.LogInformation("[CatalogAPI] Promoção atualizada com sucesso. PromocaoId: {PromocaoId}, JogoId: {JogoId}", request.PromotionId, game.Id);
        }
    }
}
