using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AdicionarPromocaoJogo
{
    public class AdicionarPromocaoJogoCommandHandler : IRequestHandler<AdicionarPromocaoJogoCommand, PromocaoResponse>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly ILogger<AdicionarPromocaoJogoCommandHandler> _logger;
        private readonly IMediator _mediator;

        public AdicionarPromocaoJogoCommandHandler(IGameRepository GameRepository, 
            ILogger<AdicionarPromocaoJogoCommandHandler> logger, IMediator mediator)
        {
            _jogoRepository = GameRepository;
            _logger = logger;
            _mediator = mediator;
        }
        public async Task<PromocaoResponse> Handle(AdicionarPromocaoJogoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para Add promoção ao Game. GameId: {GameId}, Amount: {Amount}", request.GameId, request.ValorPromocao);

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

            var valorPromocao = new Price(request.ValorPromocao);   
            Game.AddPromotion(valorPromocao, Period);
            _jogoRepository.Update(Game);

            var novaPromocao = Game.Promotions.First();

            _logger.LogInformation("[CatalogAPI] Promoção adicionada com sucesso. GameId: {GameId}, PromotionId: {PromotionId}, Amount: {Amount}", Game.Id, novaPromocao.Id, request.ValorPromocao);

            await _mediator.Publish(new PromotionAddedEvent(novaPromocao.GameId, novaPromocao.Id), cancellationToken);

            return new PromocaoResponse
            {
                GameId = Game.Id,
                DescricaoJogo = Game.Description.Value,
                NomeJogo = Game.Name.Value,
                PromotionId = novaPromocao.Id,
                ValorPromocao = novaPromocao.ValorPromocao.Amount,
                EndDate = novaPromocao.Period.EndDate,
                StartDate = novaPromocao.Period.StartDate
            };
                
        }
    }
}
