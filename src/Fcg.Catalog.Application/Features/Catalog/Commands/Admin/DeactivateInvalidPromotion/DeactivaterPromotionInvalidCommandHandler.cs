using Fcg.Catalog.Domain.Events;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateInvalidPromotion
{
    public class DesativarPromocaoInvalidaCommandHandler : IRequestHandler<DesativarPromocaoInvalidaCommand>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DesativarPromocaoInvalidaCommandHandler> _logger;
        private readonly IMediator _mediator;
        public DesativarPromocaoInvalidaCommandHandler(
            IGameRepository GameRepository,
            IUnitOfWork unitOfWork,
            ILogger<DesativarPromocaoInvalidaCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = GameRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task Handle(DesativarPromocaoInvalidaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando desativação automática de promoções expiradas/inválidas.");

            await _jogoRepository.DeactivateInvalidPromotions();
            await _unitOfWork.CommitAsync();

            await _mediator.Publish(new InvalidPromotionEvent(), cancellationToken);

            _logger.LogInformation("[CatalogAPI] Desativação automática de promoções concluída.");
        }
    }
}
