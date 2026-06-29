using Fcg.Catalogo.Application.Features.Response;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.AdicionarPromocaoJogo
{
    public class AdicionarPromocaoJogoCommandHandler : IRequestHandler<AdicionarPromocaoJogoCommand, PromocaoResponse>
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly ILogger<AdicionarPromocaoJogoCommandHandler> _logger;

        public AdicionarPromocaoJogoCommandHandler(IJogoRepository jogoRepository, ILogger<AdicionarPromocaoJogoCommandHandler> logger)
        {
            _jogoRepository = jogoRepository;
            _logger = logger;
        }
        public async Task<PromocaoResponse> Handle(AdicionarPromocaoJogoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para adicionar promoção ao jogo. JogoId: {JogoId}, Valor: {Valor}", request.JogoId, request.ValorPromocao);

            var periodo = new Periodo(request.DataInicio, request.DataFim);
            var jogo = await _jogoRepository.ObterPorId(request.JogoId);
            if (jogo == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao adicionar promoção. Jogo não encontrado. JogoId: {JogoId}", request.JogoId);
                throw new DomainException(MensagensDominio.JogoNaoEncontrado);
            }

            if (jogo.Promocoes.Any())
            {
                _logger.LogWarning("[CatalogAPI] Falha ao adicionar promoção. O jogo já possui promoções registradas. JogoId: {JogoId}", request.JogoId);
                throw new DomainException(MensagensDominio.JogoPromocoes);
            }

            var valorPromocao = new Preco(request.ValorPromocao);   
            jogo.AdicionarPromocao(valorPromocao, periodo);
            _jogoRepository.Atualizar(jogo);

            var novaPromocao = jogo.Promocoes.First();

            _logger.LogInformation("[CatalogAPI] Promoção adicionada com sucesso. JogoId: {JogoId}, PromocaoId: {PromocaoId}, Valor: {Valor}", jogo.Id, novaPromocao.Id, request.ValorPromocao);

            return new PromocaoResponse
            {
                JogoId = jogo.Id,
                DescricaoJogo = jogo.Descricao.Valor,
                NomeJogo = jogo.Nome.Valor,
                PromocaoId = novaPromocao.Id,
                ValorPromocao = novaPromocao.ValorPromocao.Valor,
                DataFim = novaPromocao.Periodo.DataFim,
                DataInicio = novaPromocao.Periodo.DataInicio
            };
                
        }
    }
}
