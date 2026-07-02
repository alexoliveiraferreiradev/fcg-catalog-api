using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.ReativarJogo
{
    public class ReativarJogoCommandHandler : IRequestHandler<ReativarJogoCommand>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReativarJogoCommandHandler> _logger;
        private readonly IMediator _mediator;
        public ReativarJogoCommandHandler(
            IGameRepository GameRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<ReativarJogoCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = GameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;   
        }

        public async Task Handle(ReativarJogoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando reativação do Game. ID: {GameId}", request.GameId);

            var Game = await _jogoRepository.GetById(request.GameId);
            if (Game == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao Reactivate Game. Game não encontrado. ID: {GameId}", request.GameId);
                throw new DomainException(MensagensDominio.JogoNaoEncontrado);
            }

            Game.Reactivate();
            _jogoRepository.Update(Game);
            await _unitOfWork.CommitAsync();

            await _mediator.Publish(new GameReactivatedEvent(Game.Id), cancellationToken);   

            _logger.LogInformation("[CatalogAPI] Game reativado com sucesso. ID: {GameId}", Game.Id);
        }
    }
}
