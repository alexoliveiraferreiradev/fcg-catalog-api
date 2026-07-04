using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.UpdateGame
{
    public class UpdateGameCommandHandler : IRequestHandler<UpdateGameCommand, GameResponse>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateGameCommandHandler> _logger;
        private readonly IMediator _mediator;

        public UpdateGameCommandHandler(
            IGameRepository gameRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateGameCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = gameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<GameResponse> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para atualizar Jogo. ID: {JogoId}, NovoNome: {NovoNome}", request.GameId, request.NewName);

            var game = await _jogoRepository.GetById(request.GameId);
            if (game == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao atualizar Jogo. Jogo não encontrado. ID: {JogoId}", request.GameId);
                throw new DomainException(DomainMessages.GameNotFound);
            }

            var novoNome = new Name(request.NewName);
            var novaDescricao = new Description(request.NewDescription);
            var novoPreco = new Price(request.NewPrice);

            game.Update(novoNome, novaDescricao, novoPreco, request.NewGenre);
            _jogoRepository.Update(game);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[CatalogAPI] Jogo atualizado com sucesso. ID: {JogoId}", game.Id);

            await _mediator.Publish(new GameUpdatedEvent(request.GameId), cancellationToken);

            return new GameResponse
            {
                Id = game.Id,
                Name = game.Name.Value,
                Description = game.Description.Value,
                OriginalPrice = game.BasePrice.Amount,
                Genre = game.Genre,
                IsActive = game.IsActive
            };
        }
    }
}
