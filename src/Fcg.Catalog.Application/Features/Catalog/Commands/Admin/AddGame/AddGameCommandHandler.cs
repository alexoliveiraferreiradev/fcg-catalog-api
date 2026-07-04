using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddGame
{
    public class AddGameCommandHandler : IRequestHandler<AddGameCommand, GameResponse>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly ILogger<AddGameCommandHandler> _logger;        
        private readonly IMediator _mediator;
        private readonly IUnitOfWork _unitOfWork;

        public AddGameCommandHandler(IGameRepository gameRepository, ILogger<AddGameCommandHandler> logger,
             IMediator mediator, IUnitOfWork unitOfWork)
        {
            _jogoRepository = gameRepository;
            _logger = logger;
            _mediator = mediator;
            _unitOfWork = unitOfWork;
        }
        public async Task<GameResponse> Handle(AddGameCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para adicionar novo jogo. Nome jogo: {Nome}, Gênero: {Genre}, Preço: {Preco}", request.Name, request.Genre, request.Price);

            var nomeJaExistente = await CheckNameDuplicity(request.Name);
            if(nomeJaExistente)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao adicionar novo jogo. Já existe um jogo cadastrado com o Nome: {Nome}", request.Name);
                throw new DomainException(DomainMessages.GameNameAlreadyExists);
            }
            var price = new Price(request.Price);
            var gameName = new Name(request.Name);
            var gameDescription = new Description(request.Description);
            var game = new Game(gameName, gameDescription, price, request.Genre);
            _jogoRepository.Add(game);
           
            _logger.LogInformation("[CatalogAPI] Jogo adicionado com sucesso ao repositório. ID: {JogoId}, Nome: {Nome}", game.Id, request.Name);

            await _mediator.Publish(new GameAddedEvent(game.Id),cancellationToken);

            await _unitOfWork.CommitAsync();

            return new GameResponse
            {
                Id = game.Id,
                Name = game.Name.Value,
                Description = game.Description.Value,
                OriginalPrice = game.BasePrice.Amount,
                Genre = game.Genre
            };
        }

        public async Task<bool> CheckNameDuplicity(string gameName)
        {
            return await _jogoRepository.GameExistsWithName(gameName);
        }
    }
}
