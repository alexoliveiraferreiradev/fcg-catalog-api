using Fcg.Catalogo.Application.Features.Response;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
using MassTransit;
using MediatR;
using System.IO.Pipes;

namespace Fcg.Catalogo.Application.Features.Pedidos.Commands.RealizarPedido
{
    public class RealizarPedidoCommandHandler : IRequestHandler<RealizarPedidoCommand, bool>
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBibliotecaRepository _bibliotecaRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RealizarPedidoCommandHandler(
            IJogoRepository jogoRepository, 
            IPublishEndpoint publishEndpoint, 
            IBibliotecaRepository bibliotecaRepository,
            IUnitOfWork unitOfWork)
        {
            _jogoRepository = jogoRepository;
            _publishEndpoint = publishEndpoint;
            _bibliotecaRepository = bibliotecaRepository;   
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RealizarPedidoCommand request, CancellationToken cancellationToken)
        {
            var jogos = await _jogoRepository.ObterJogosPorIds(request.JogosIds);
            var idsEncontrados = jogos.Select(j => j.Id);
            var idsInexistentes = request.JogosIds.Except(idsEncontrados);
            var jogosJaPossuidos = await _bibliotecaRepository.ObterJogosAdquiridosPorUsuario(request.UsuarioId);

            if (idsInexistentes.Any())
            {
                var idsFormatados = string.Join(", ", idsInexistentes);
                throw new DomainException($"A compra foi cancelada. Os seguintes IDs de jogos não foram encontrados no catálogo: {idsFormatados}");
            }

            decimal precoTotal = 0;
            foreach (var jogo in jogos)
            {
                if (jogosJaPossuidos.Contains(jogo.Id))
                {
                    throw new DomainException($"O usuário já possui o jogo {jogo.Nome.Valor} em sua biblioteca.");
                }

                if (!jogo.Ativo)
                {
                    throw new DomainException($"O jogo {jogo.Nome.Valor} não está mais disponível para aquisição.");
                }

                precoTotal += jogo.ObterPrecoAtual().Valor;
            }

            await _publishEndpoint.Publish(new OrderPlacedEvent(
                OrderId: Guid.NewGuid(),
                UserId: request.UsuarioId,
                JogosIds: request.JogosIds,
                PrecoTotal: precoTotal), cancellationToken);
            
            // Necessário chamar o commit para persistir a mensagem na tabela de Outbox do EF e enviá-la ao RabbitMQ
            await _unitOfWork.CommitAsync();
            
            return true;
        }
    }
}
