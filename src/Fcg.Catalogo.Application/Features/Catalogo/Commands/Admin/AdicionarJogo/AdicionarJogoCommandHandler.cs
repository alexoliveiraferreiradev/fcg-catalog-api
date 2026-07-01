using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Application.Features.Catalogo.EventHandlers;
using Fcg.Catalogo.Application.Features.Response;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Events;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.AdicionarJogo
{
    public class AdicionarJogoCommandHandler : IRequestHandler<AdicionarJogoCommand, JogoResponse>
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly ILogger<AdicionarJogoCommandHandler> _logger;        
        private readonly IMediator _mediator;

        public AdicionarJogoCommandHandler(IJogoRepository jogoRepository, ILogger<AdicionarJogoCommandHandler> logger,
             IMediator mediator)
        {
            _jogoRepository = jogoRepository;
            _logger = logger;            
            _mediator = mediator;
        }
        public async Task<JogoResponse> Handle(AdicionarJogoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para adicionar novo jogo. Nome: {Nome}, Genero: {Genero}, Preco: {Preco}", request.Nome, request.Genero, request.Preco);

            var nomeJaExistente = await VerificaDuplicidadeNome(request.Nome);
            if(nomeJaExistente)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao adicionar jogo. Já existe um jogo cadastrado com o nome: {Nome}", request.Nome);
                throw new DomainException(MensagensDominio.JogoMesmoNomeExistente);
            }
            var preco = new Preco(request.Preco);
            var nomeJogo = new Nome(request.Nome);
            var descricaoJogo = new Descricao(request.Descricao);
            var jogo = new Jogo(nomeJogo, descricaoJogo, preco, request.Genero);
            _jogoRepository.Adicionar(jogo);
           
            _logger.LogInformation("[CatalogAPI] Jogo adicionado com sucesso ao repositório. ID: {JogoId}, Nome: {Nome}", jogo.Id, request.Nome);

            await _mediator.Publish(new JogoAdicionadoEvent(jogo.Id),cancellationToken);

            return new JogoResponse
            {
                Id = jogo.Id,
                Nome = jogo.Nome.Valor,
                Descricao = jogo.Descricao.Valor,
                PrecoOriginal = jogo.PrecoBase.Valor,
                Genero = jogo.Genero
            };
        }

        public async Task<bool> VerificaDuplicidadeNome(string nomeJogo)
        {
            return await _jogoRepository.ExisteJogoComNome(nomeJogo);
        }
    }
}
