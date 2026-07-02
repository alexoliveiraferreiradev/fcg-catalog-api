using Fcg.Catalog.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using MassTransit;
using MediatR;
using System.IO.Pipes;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalog.Application.Features.Pedidos.Commands.RealizarPedido
{
    public class RealizarPedidoCommandHandler : IRequestHandler<RealizarPedidoCommand, bool>
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBibliotecaRepository _bibliotecaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RealizarPedidoCommandHandler> _logger;

        public RealizarPedidoCommandHandler(
            IJogoRepository jogoRepository, 
            IPublishEndpoint publishEndpoint, 
            IBibliotecaRepository bibliotecaRepository,
            IUnitOfWork unitOfWork,
            ILogger<RealizarPedidoCommandHandler> logger)
        {
            _jogoRepository = jogoRepository;
            _publishEndpoint = publishEndpoint;
            _bibliotecaRepository = bibliotecaRepository;   
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(RealizarPedidoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI]  Iniciando processamento do pedido para o usuário: {UsuarioId}", request.UsuarioId);

            var jogos = await _jogoRepository.ObterJogosPorIds(request.JogosIds);
            var idsEncontrados = jogos.Select(j => j.Id);
            var idsInexistentes = request.JogosIds.Except(idsEncontrados);
            var jogosJaPossuidos = await _bibliotecaRepository.ObterJogosAdquiridosPorUsuario(request.UsuarioId);

            if (idsInexistentes.Any())
            {
                var idsFormatados = string.Join(", ", idsInexistentes);
                _logger.LogWarning("[CatalogAPI]  Pedido negado. Os seguintes jogos não foram encontrados: {IdsInexistentes}", idsFormatados);
                throw new DomainException($"A compra foi cancelada. Os seguintes IDs de jogos não foram encontrados no catálogo: {idsFormatados}");
            }

            decimal precoTotal = 0;
            foreach (var jogo in jogos)
            {
                if (jogosJaPossuidos.Contains(jogo.Id))
                {
                    _logger.LogWarning("[CatalogAPI]  Pedido negado. Usuário {UsuarioId} já possui o jogo {JogoId}", request.UsuarioId, jogo.Id);
                    throw new DomainException($"O usuário já possui o jogo {jogo.Nome.Valor} em sua biblioteca.");
                }

                if (!jogo.Ativo)
                {
                    _logger.LogWarning("[CatalogAPI]  Pedido negado. Jogo {JogoId} inativo", jogo.Id);
                    throw new DomainException($"O jogo {jogo.Nome.Valor} não está mais disponível para aquisição.");
                }

                precoTotal += jogo.ObterPrecoAtual().Valor;
            }

            var orderId = Guid.NewGuid();
            _logger.LogInformation("[CatalogAPI]  Validações concluídas. Publicando OrderPlacedEvent (OrderId: {OrderId}, Total: {PrecoTotal})", orderId, precoTotal);

            await _publishEndpoint.Publish(new OrderPlacedEvent(
                OrderId: orderId,
                UserId: request.UsuarioId,
                EmailUsuario: request.EmailUsuario,
                NomeUsuario: request.NomeUsuario,
                JogosIds: request.JogosIds,
                PrecoTotal: precoTotal), cancellationToken);
            
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[Pedidos] Processamento do pedido {OrderId} finalizado com sucesso.", orderId);
            
            return true;
        }
    }
}
