using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateGame
{
    public class DeactivateGameCommandHandler : IRequestHandler<DeactivateGameCommand>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DeactivateGameCommandHandler> _logger;
        private readonly IMediator _mediator;

        public DeactivateGameCommandHandler(
            IGameRepository GameRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<DeactivateGameCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = GameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;   
        }

        public async Task Handle(DeactivateGameCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando desativação de Jogo. ID: {JogoId}", request.GameId);

            var Game = await _jogoRepository.GetById(request.GameId);
            if (Game == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao desativar Jogo. Jogo não encontrado. ID: {JogoId}", request.GameId);
                throw new DomainException(DomainMessages.GameNotFound);
            }

            Game.Deactivate();
            _jogoRepository.Update(Game);
            await _unitOfWork.CommitAsync();

            await _mediator.Publish(new GameDeactivatedEvent(Game.Id), cancellationToken);

            _logger.LogInformation("[CatalogAPI] Jogo desativado com sucesso. ID: {JogoId}", Game.Id);
        }
    }
}
