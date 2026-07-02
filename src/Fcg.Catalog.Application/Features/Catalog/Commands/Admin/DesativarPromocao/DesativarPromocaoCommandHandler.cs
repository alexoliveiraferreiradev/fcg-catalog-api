using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DesativarPromocao
{
    public class DesativarPromocaoCommandHandler : IRequestHandler<DesativarPromocaoCommand>
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DesativarPromocaoCommandHandler> _logger;
        private readonly IMediator _mediator;
        public DesativarPromocaoCommandHandler(
            IJogoRepository jogoRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<DesativarPromocaoCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = jogoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;   
        }

        public async Task Handle(DesativarPromocaoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para desativar promoção. PromocaoId: {PromocaoId}", request.PromocaoId);

            var promocao = await _jogoRepository.ObterPromocaoPorId(request.PromocaoId);
            if (promocao == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao desativar promoção. Promoção não encontrada. PromocaoId: {PromocaoId}", request.PromocaoId);
                throw new DomainException(MensagensDominio.PromocaoNaoEncontrada);
            }

            var jogo = await _jogoRepository.ObterPorId(promocao.JogoId);
            if (jogo == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao desativar promoção. Jogo não encontrado. JogoId: {JogoId}", promocao.JogoId);
                throw new DomainException(MensagensDominio.JogoNaoEncontrado);
            }

            jogo.DesativarPromocao(request.PromocaoId);
            _jogoRepository.Atualizar(jogo);
            await _unitOfWork.CommitAsync();

            await _mediator.Publish(new PromocaoDesativadaEvent(jogo.Id,request.PromocaoId), cancellationToken);

            _logger.LogInformation("[CatalogAPI] Promoção desativada com sucesso. PromocaoId: {PromocaoId}, JogoId: {JogoId}", request.PromocaoId, promocao.JogoId);
        }
    }
}
