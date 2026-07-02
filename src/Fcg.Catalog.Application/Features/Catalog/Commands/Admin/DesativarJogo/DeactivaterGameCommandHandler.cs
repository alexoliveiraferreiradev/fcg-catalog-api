using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DesativarJogo
{
    public class DesativarJogoCommandHandler : IRequestHandler<DesativarJogoCommand>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DesativarJogoCommandHandler> _logger;
        private readonly IMediator _mediator;

        public DesativarJogoCommandHandler(
            IGameRepository GameRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<DesativarJogoCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = GameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;   
        }

        public async Task Handle(DesativarJogoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando desativação de Game. ID: {GameId}", request.GameId);

            var Game = await _jogoRepository.GetById(request.GameId);
            if (Game == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao Deactivate Game. Game não encontrado. ID: {GameId}", request.GameId);
                throw new DomainException(DomainMessages.GameNotFound);
            }

            Game.Deactivate();
            _jogoRepository.Update(Game);
            await _unitOfWork.CommitAsync();

            await _mediator.Publish(new GameDeactivatedEvent(Game.Id), cancellationToken);

            _logger.LogInformation("[CatalogAPI] Game desativado com sucesso. ID: {GameId}", Game.Id);
        }
    }
}
