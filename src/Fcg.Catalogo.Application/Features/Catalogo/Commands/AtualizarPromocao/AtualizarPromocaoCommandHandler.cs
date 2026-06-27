using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Domain.Resources;
using Fcg.Catalogo.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.AtualizarPromocao
{
    public class AtualizarPromocaoCommandHandler : IRequestHandler<AtualizarPromocaoCommand>
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AtualizarPromocaoCommandHandler> _logger;

        public AtualizarPromocaoCommandHandler(
            IJogoRepository jogoRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<AtualizarPromocaoCommandHandler> logger)
        {
            _jogoRepository = jogoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(AtualizarPromocaoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para atualizar promoção. PromocaoId: {PromocaoId}, JogoId: {JogoId}, NovoValor: {NovoValor}", request.PromocaoId, request.JogoId, request.NovoValorPromocao);

            var jogo = await _jogoRepository.ObterPorId(request.JogoId);
            if (jogo == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao atualizar promoção. Jogo não encontrado. JogoId: {JogoId}", request.JogoId);
                throw new DomainException(MensagensDominio.JogoNaoEncontrado);
            }

            var novoPreco = new Preco(request.NovoValorPromocao);

            jogo.AlteraPromocao(request.PromocaoId, novoPreco, request.NovaDataFim);
            _jogoRepository.Atualizar(jogo);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[CatalogAPI] Promoção atualizada com sucesso. PromocaoId: {PromocaoId}, JogoId: {JogoId}", request.PromocaoId, request.JogoId);
        }
    }
}
