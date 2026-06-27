using Fcg.Catalogo.Application.Features.Response;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Domain.Resources;
using Fcg.Catalogo.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.AdicionarJogo
{
    public class AdicionarJogoCommandHandler : IRequestHandler<AdicionarJogoCommand, JogosResponse>
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly ILogger<AdicionarJogoCommandHandler> _logger;

        public AdicionarJogoCommandHandler(IJogoRepository jogoRepository, ILogger<AdicionarJogoCommandHandler> logger)
        {
            _jogoRepository = jogoRepository;
            _logger = logger;
        }
        public async Task<JogosResponse> Handle(AdicionarJogoCommand request, CancellationToken cancellationToken)
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

            return new JogosResponse
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
