using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddPromotionGame
{
    public class AddPromotionGameCommandHandler : IRequestHandler<AddPromotionGameCommand, PromotionResponse>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly ILogger<AddPromotionGameCommandHandler> _logger;
        private readonly IMediator _mediator;

        public AddPromotionGameCommandHandler(IGameRepository GameRepository, 
            ILogger<AddPromotionGameCommandHandler> logger, IMediator mediator)
        {
            _jogoRepository = GameRepository;
            _logger = logger;
            _mediator = mediator;
        }
        public async Task<PromotionResponse> Handle(AddPromotionGameCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para Add promoção ao Game. GameId: {GameId}, Amount: {Amount}", request.GameId, request.PromotionValue);

            var Period = new Period(request.StartDate, request.EndDate);
            var Game = await _jogoRepository.GetById(request.GameId);
            if (Game == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao Add promoção. Game não encontrado. GameId: {GameId}", request.GameId);
                throw new DomainException(DomainMessages.GameNotFound);
            }

            if (Game.Promotions.Any())
            {
                _logger.LogWarning("[CatalogAPI] Falha ao Add promoção. O Game já possui promoções registradas. GameId: {GameId}", request.GameId);
                throw new DomainException(DomainMessages.GameWithPromotions);
            }

            var valorPromocao = new Price(request.PromotionValue);   
            Game.AddPromotion(valorPromocao, Period);
            _jogoRepository.Update(Game);

            var novaPromocao = Game.Promotions.First();

            _logger.LogInformation("[CatalogAPI] Promoção adicionada com sucesso. GameId: {GameId}, PromotionId: {PromotionId}, Amount: {Amount}", Game.Id, novaPromocao.Id, request.PromotionValue);

            await _mediator.Publish(new PromotionAddedEvent(novaPromocao.GameId, novaPromocao.Id), cancellationToken);

            return new PromotionResponse
            {
                GameId = Game.Id,
                GameDescription = Game.Description.Value,
                GameName = Game.Name.Value,
                PromotionId = novaPromocao.Id,
                ValorPromocao = novaPromocao.ValorPromocao.Amount,
                EndDate = novaPromocao.Period.EndDate,
                StartDate = novaPromocao.Period.StartDate
            };
                
        }
    }
}
