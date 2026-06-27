using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Domain.Resources;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.ReativarJogo
{
    public class ReativarJogoCommandHandler : IRequestHandler<ReativarJogoCommand>
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ReativarJogoCommandHandler> _logger;

        public ReativarJogoCommandHandler(
            IJogoRepository jogoRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<ReativarJogoCommandHandler> logger)
        {
            _jogoRepository = jogoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(ReativarJogoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando reativação do jogo. ID: {JogoId}", request.JogoId);

            var jogo = await _jogoRepository.ObterPorId(request.JogoId);
            if (jogo == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao reativar jogo. Jogo não encontrado. ID: {JogoId}", request.JogoId);
                throw new DomainException(MensagensDominio.JogoNaoEncontrado);
            }

            jogo.Reativar();
            _jogoRepository.Atualizar(jogo);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[CatalogAPI] Jogo reativado com sucesso. ID: {JogoId}", jogo.Id);
        }
    }
}
