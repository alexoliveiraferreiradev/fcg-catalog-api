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

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AtualizarJogo
{
    public class UpdateGameCommandHandler : IRequestHandler<UpdateGameCommand, JogoResponse>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UpdateGameCommandHandler> _logger;
        private readonly IMediator _mediator;

        public UpdateGameCommandHandler(
            IGameRepository GameRepository,
            IUnitOfWork unitOfWork,
            ILogger<UpdateGameCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = GameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<JogoResponse> Handle(UpdateGameCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para Update Game. ID: {GameId}, NovoNome: {NovoNome}", request.GameId, request.NovoNome);

            var Game = await _jogoRepository.GetById(request.GameId);
            if (Game == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao Update Game. Game não encontrado. ID: {GameId}", request.GameId);
                throw new DomainException(DomainMessages.GameNotFound);
            }

            var novoNome = new Name(request.NovoNome);
            var novaDescricao = new Description(request.NovaDescricao);
            var novoPreco = new Price(request.NovoPreco);

            Game.Update(novoNome, novaDescricao, novoPreco, request.NovoGenero);
            _jogoRepository.Update(Game);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[CatalogAPI] Game atualizado com sucesso. ID: {GameId}", Game.Id);

            await _mediator.Publish(new GameUpdatedEvent(request.GameId), cancellationToken);

            return new JogoResponse
            {
                Id = Game.Id,
                Name = Game.Name.Value,
                Description = Game.Description.Value,
                PrecoOriginal = Game.BasePrice.Amount,
                Genre = Game.Genre,
                IsActive = Game.IsActive
            };
        }
    }
}
