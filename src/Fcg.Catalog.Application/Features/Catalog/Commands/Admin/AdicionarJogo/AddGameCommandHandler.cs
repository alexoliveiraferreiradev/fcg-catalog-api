using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Catalog.Application.Features.Catalog.EventHandlers;
using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Catalog.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AdicionarJogo
{
    public class AddGameCommandHandler : IRequestHandler<AddGameCommand, JogoResponse>
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
        public async Task<JogoResponse> Handle(AddGameCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para Add novo Game. Name: {Name}, Genre: {Genre}, Price: {Price}", request.Name, request.Genre, request.Price);

            var nomeJaExistente = await VerificaDuplicidadeNome(request.Name);
            if(nomeJaExistente)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao Add Game. Já existe um Game cadastrado com o Name: {Name}", request.Name);
                throw new DomainException(DomainMessages.GameNameAlreadyExists);
            }
            var Price = new Price(request.Price);
            var nomeJogo = new Name(request.Name);
            var descricaoJogo = new Description(request.Description);
            var Game = new Game(nomeJogo, descricaoJogo, Price, request.Genre);
            _jogoRepository.Add(Game);
           
            _logger.LogInformation("[CatalogAPI] Game adicionado com sucesso ao repositório. ID: {GameId}, Name: {Name}", Game.Id, request.Name);

            await _mediator.Publish(new GameAddedEvent(Game.Id),cancellationToken);

            return new JogoResponse
            {
                Id = Game.Id,
                Name = Game.Name.Value,
                Description = Game.Description.Value,
                PrecoOriginal = Game.BasePrice.Amount,
                Genre = Game.Genre
            };
        }

        public async Task<bool> VerificaDuplicidadeNome(string nomeJogo)
        {
            return await _jogoRepository.GameExistsWithName(nomeJogo);
        }
    }
}
