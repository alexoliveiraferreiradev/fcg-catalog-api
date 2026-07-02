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
            IGameRepository GameRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<UpdatePromotionCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = GameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;   
        }

        public async Task Handle(UpdatePromotionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para Update promoção. PromotionId: {PromotionId}, GameId: {GameId}, NovoValor: {NovoValor}", request.PromotionId, request.GameId, request.NovoValorPromocao);

            var Game = await _jogoRepository.GetById(request.GameId);
            if (Game == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao Update promoção. Game não encontrado. GameId: {GameId}", request.GameId);
                throw new DomainException(DomainMessages.GameNotFound);
            }

            var novoPreco = new Price(request.NovoValorPromocao);

            Game.UpdatePromotion(request.PromotionId, novoPreco, request.NovaDataFim);
            _jogoRepository.Update(Game);
            await _unitOfWork.CommitAsync();

            var novaPromocao = Game.Promotions.First();
                        
            await _mediator.Publish(new PromotionUpdatedEvent(Game.Id, novaPromocao.Id),cancellationToken);

            _logger.LogInformation("[CatalogAPI] Promoção atualizada com sucesso. PromotionId: {PromotionId}, GameId: {GameId}", request.PromotionId, request.GameId);
        }
    }
}
