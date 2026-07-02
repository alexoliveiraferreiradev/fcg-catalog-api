using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using MassTransit;
using MediatR;
using System.IO.Pipes;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, bool>
    {
        private readonly IGameRepository _jogoRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILibraryRepository _bibliotecaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PlaceOrderCommandHandler> _logger;

        public PlaceOrderCommandHandler(
            IGameRepository GameRepository, 
            IPublishEndpoint publishEndpoint, 
            ILibraryRepository LibraryRepository,
            IUnitOfWork unitOfWork,
            ILogger<PlaceOrderCommandHandler> logger)
        {
            _jogoRepository = GameRepository;
            _publishEndpoint = publishEndpoint;
            _bibliotecaRepository = LibraryRepository;   
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI]  Iniciando processamento do Order para o usuário: {UserId}", request.UserId);

            var Games = await _jogoRepository.GetGamesByIds(request.JogosIds);
            var idsEncontrados = Games.Select(j => j.Id);
            var idsInexistentes = request.JogosIds.Except(idsEncontrados);
            var jogosJaPossuidos = await _bibliotecaRepository.GetPurchasedGamesByUser(request.UserId);

            if (idsInexistentes.Any())
            {
                var idsFormatados = string.Join(", ", idsInexistentes);
                _logger.LogWarning("[CatalogAPI]  Order negado. Os seguintes Games não foram encontrados: {IdsInexistentes}", idsFormatados);
                throw new DomainException($"A compra foi cancelada. Os seguintes IDs de Games não foram encontrados no catálogo: {idsFormatados}");
            }

            decimal precoTotal = 0;
            foreach (var Game in Games)
            {
                if (jogosJaPossuidos.Contains(Game.Id))
                {
                    _logger.LogWarning("[CatalogAPI]  Order negado. Usuário {UserId} já possui o Game {GameId}", request.UserId, Game.Id);
                    throw new DomainException($"O usuário já possui o Game {Game.Name.Value} em sua Library.");
                }

                if (!Game.IsActive)
                {
                    _logger.LogWarning("[CatalogAPI]  Order negado. Game {GameId} inativo", Game.Id);
                    throw new DomainException($"O Game {Game.Name.Value} não está mais disponível para aquisição.");
                }

                precoTotal += Game.GetCurrentPrice().Amount;
            }

            var orderId = Guid.NewGuid();
            _logger.LogInformation("[CatalogAPI]  Validações concluídas. Publicando OrderPlacedEvent (OrderId: {OrderId}, Total: {PrecoTotal})", orderId, precoTotal);

            await _publishEndpoint.Publish(new OrderPlacedEvent(
                OrderId: orderId,
                UserId: request.UserId,
                EmailUsuario: request.EmailUsuario,
                NomeUsuario: request.NomeUsuario,
                JogosIds: request.JogosIds,
                PrecoTotal: precoTotal), cancellationToken);
            
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[Orders] Processamento do Order {OrderId} Completed com sucesso.", orderId);
            
            return true;
        }
    }
}
