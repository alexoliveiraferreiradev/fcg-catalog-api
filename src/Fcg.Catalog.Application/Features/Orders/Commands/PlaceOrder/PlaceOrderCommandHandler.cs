using Fcg.Catalog.Domain.Entities;
using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Orders.Commands.PlaceOrder
{
    public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, bool>
    {
        
        private readonly IGameRepository _gameRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILibraryRepository _libraryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PlaceOrderCommandHandler> _logger;
        private readonly IOrderRepository _orderRepository;

        public PlaceOrderCommandHandler(
            IPublishEndpoint publishEndpoint,
            ILibraryRepository libraryRepository,
            IUnitOfWork unitOfWork,
            IOrderRepository orderRepository,
            IGameRepository gameRepository,
            ILogger<PlaceOrderCommandHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _libraryRepository = libraryRepository;
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _gameRepository = gameRepository;
        }

        public async Task<bool> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI]  Iniciando processamento do pedido para o usuário: {UsuarioId}", request.UserId);
            var orderUser = new Order(request.UserId);
            var games = await _gameRepository.GetGamesByIds(request.JogosIds);
            var idsEncontrados = games.Select(j => j.Id);
            var idsInexistentes = request.JogosIds.Except(idsEncontrados);
            var jogosJaPossuidos = await _libraryRepository.GetPurchasedGamesByUser(request.UserId);

            if (idsInexistentes.Any())
            {
                var idsFormatados = string.Join(", ", idsInexistentes);
                _logger.LogWarning("[CatalogAPI]  pedido negado. Os seguintes Jogos não foram encontrados: {IdsInexistentes}", idsFormatados);
                throw new DomainException($"A compra foi cancelada. Os seguintes IDs de Games não foram encontrados no catálogo: {idsFormatados}");
            }

            decimal precoTotal = 0;
            foreach (var game in games)
            {
                if (jogosJaPossuidos.Contains(game.Id))
                {
                    _logger.LogWarning("[CatalogAPI]  pedido negado. Usuário {UsuarioId} já possui o Jogo {JogoId}", request.UserId, game.Id);
                    throw new DomainException($"O usuário já possui o Game {game.Name} em sua Library.");
                }

                if (!game.IsActive)
                {
                    _logger.LogWarning("[CatalogAPI]  pedido negado. Jogo {JogoId} inativo", game.Id);
                    throw new DomainException($"O Game {game.Name} não está mais disponível para aquisição.");
                }

                precoTotal += game.GetCurrentPrice().Amount;
                orderUser.AddItem(game.Id, game.Name.Value, game.BasePrice.Amount);
            }

            orderUser.MakeOrder();

            _orderRepository.Add(orderUser);    
            
            _logger.LogInformation("[CatalogAPI]  Validações concluídas. Publicando OrderPlacedEvent (PedidoId: {PedidoId}, Total: {PrecoTotal})", orderUser.Id, precoTotal);

            await _publishEndpoint.Publish(new OrderPlacedEvent(
                OrderId: orderUser.Id,
                UserId: request.UserId,
                GameIds: request.JogosIds,
                AmountPrice: precoTotal), cancellationToken);
            
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[Pedidos] Processamento do pedido {PedidoId} concluído com sucesso.", orderUser.Id);
            
            return true;
        }
    }
}
