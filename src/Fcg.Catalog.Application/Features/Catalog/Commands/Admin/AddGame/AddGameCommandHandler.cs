using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
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

        public AddGameCommandHandler(IGameRepository GameRepository, ILogger<AddGameCommandHandler> logger,
             IMediator mediator)
        {
            _jogoRepository = GameRepository;
            _logger = logger;            
            _mediator = mediator;
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
            var Price = new Price(request.Price);
            var GameName = new Name(request.Name);
            var GameDescription = new Description(request.Description);
            var Game = new Game(GameName, GameDescription, Price, request.Genre);
            _jogoRepository.Add(Game);
           
            _logger.LogInformation("[CatalogAPI] Jogo adicionado com sucesso ao repositório. ID: {JogoId}, Nome: {Nome}", Game.Id, request.Name);

            await _mediator.Publish(new GameAddedEvent(Game.Id),cancellationToken);

            return new GameResponse
            {
                Id = Game.Id,
                Name = Game.Name.Value,
                Description = Game.Description.Value,
                OriginalPrice = Game.BasePrice.Amount,
                Genre = Game.Genre
            };
        }

        public async Task<bool> CheckNameDuplicity(string GameName)
        {
            return await _jogoRepository.GameExistsWithName(GameName);
        }
    }
}
