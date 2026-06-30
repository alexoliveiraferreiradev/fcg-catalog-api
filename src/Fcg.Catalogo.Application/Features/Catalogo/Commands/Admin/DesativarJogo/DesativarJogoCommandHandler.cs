using Fcg.Catalogo.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.DesativarJogo
{
    public class DesativarJogoCommandHandler : IRequestHandler<DesativarJogoCommand>
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DesativarJogoCommandHandler> _logger;

        public DesativarJogoCommandHandler(
            IJogoRepository jogoRepository, 
            IUnitOfWork unitOfWork, 
            ILogger<DesativarJogoCommandHandler> logger)
        {
            _jogoRepository = jogoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task Handle(DesativarJogoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando desativação de jogo. ID: {JogoId}", request.JogoId);

            var jogo = await _jogoRepository.ObterPorId(request.JogoId);
            if (jogo == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao desativar jogo. Jogo não encontrado. ID: {JogoId}", request.JogoId);
                throw new DomainException(MensagensDominio.JogoNaoEncontrado);
            }

            jogo.Desativar();
            _jogoRepository.Atualizar(jogo);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[CatalogAPI] Jogo desativado com sucesso. ID: {JogoId}", jogo.Id);
        }
    }
}
