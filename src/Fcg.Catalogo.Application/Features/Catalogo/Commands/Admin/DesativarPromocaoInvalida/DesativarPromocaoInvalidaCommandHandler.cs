using Fcg.Catalogo.Domain.Events;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Core.Abstractions.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.DesativarPromocaoInvalida
{
    public class DesativarPromocaoInvalidaCommandHandler : IRequestHandler<DesativarPromocaoInvalidaCommand>
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DesativarPromocaoInvalidaCommandHandler> _logger;
        private readonly IMediator _mediator;
        public DesativarPromocaoInvalidaCommandHandler(
            IJogoRepository jogoRepository,
            IUnitOfWork unitOfWork,
            ILogger<DesativarPromocaoInvalidaCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = jogoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(DesativarPromocaoInvalidaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando desativação automática de promoções expiradas/inválidas.");

            await _jogoRepository.DesativaPromocoesInvalidas();
            await _unitOfWork.CommitAsync();

            await _mediator.Publish(new PromocaoInvalidaEvent(), cancellationToken);

            _logger.LogInformation("[CatalogAPI] Desativação automática de promoções concluída.");
        }
    }
}
