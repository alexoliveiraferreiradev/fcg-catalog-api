using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.ReactivateGame
{
    public class ReactivateGameCommandHandler : IRequestHandler<ReactivateGameCommand>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReactivateGameCommandHandler> _logger;
        private readonly IMediator _mediator;
        public ReactivateGameCommandHandler(
            IGameRepository gameRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<ReactivateGameCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = gameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;   
        }

        public async Task Handle(ReactivateGameCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando reativação do Jogo. ID: {JogoId}", request.GameId);

            var game = await _jogoRepository.GetById(request.GameId);
            if (game == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao reativar Jogo. Jogo não encontrado. ID: {JogoId}", request.GameId);
                throw new DomainException(DomainMessages.GameNotFound);
            }

            game.Reactivate();
            _jogoRepository.Update(game);
            await _unitOfWork.CommitAsync();

            await _mediator.Publish(new GameReactivatedEvent(game.Id), cancellationToken);   

            _logger.LogInformation("[CatalogAPI] Jogo reativado com sucesso. ID: {JogoId}", game.Id);
        }
    }
}
