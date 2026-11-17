using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Players.RatingGame
{
    public class RatingGameCommandHandler : IRequestHandler<RatingGameCommand>
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGamesRatingRepository _gamesRatingRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly ILogger<RatingGameCommandHandler> _logger;

        public RatingGameCommandHandler(IGameRepository gameRepository,
            IGamesRatingRepository gamesRatingRepository,
            IUnitOfWork unitOfWork,
            IMediator mediator,
            ILogger<RatingGameCommandHandler> logger)
        {
            _gameRepository = gameRepository;
            _gamesRatingRepository = gamesRatingRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Handle(RatingGameCommand request, CancellationToken cancellationToken)
        {
            // Vem de um evento: lançar exceção aqui só faria a mensagem ir e voltar até a fila de erro.
            var game = await _gameRepository.GetById(request.GameId);
            if (game is null)
            {
                _logger.LogWarning("[CatalogAPI] Avaliação ignorada: jogo {GameId} não encontrado.", request.GameId);
                return;
            }

            var newRating = new Avaliation(request.Rating);
            var userRating = await _gamesRatingRepository.GetByGameAndUser(request.GameId, request.UserId, cancellationToken);

            double sumDelta;
            int countDelta;
            if (userRating is null)
            {
                _gamesRatingRepository.Add(new GamesRating(request.GameId, request.UserId, newRating, request.OccurredAt));
                sumDelta = newRating.Value;
                countDelta = 1;
            }
            else
            {
                var previousRating = userRating.Rating;
                if (!userRating.UpdateRating(newRating, request.OccurredAt))
                {
                    _logger.LogInformation("[CatalogAPI] Avaliação antiga ignorada. Jogo {GameId}, usuário {UserId}.", request.GameId, request.UserId);
                    return;
                }

                _gamesRatingRepository.Update(userRating);
                sumDelta = newRating.Value - previousRating.Value;
                countDelta = 0;
            }

            // Incremento atômico no banco, na mesma transação do outbox do consumer que grava a nota do usuário.
            await _gameRepository.ApplyRatingAsync(game.Id, sumDelta, countDelta, cancellationToken);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[CatalogAPI] Avaliação registrada. Jogo {GameId}, usuário {UserId}, nota {Rating}.",
                game.Id, request.UserId, newRating.Value);

            await _mediator.Publish(new GameUpdatedEvent(game.Id), cancellationToken);
        }
    }
}
