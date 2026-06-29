using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.MessageContracts;
using MassTransit;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.AcessoGeral.AdquirirJogo
{
    public class AdquirirJogoCommandHandler : IRequestHandler<AdquirirJogoCommand, bool>
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly IBibliotecaRepository _bibliotecaRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        public AdquirirJogoCommandHandler(IJogoRepository jogoRepository, IBibliotecaRepository bibliotecaRepository, IPublishEndpoint publishEndpoint)
        {
            _jogoRepository = jogoRepository;
            _bibliotecaRepository = bibliotecaRepository;
            _publishEndpoint = publishEndpoint;
        }


        public async Task<bool> Handle(AdquirirJogoCommand request, CancellationToken cancellationToken)
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
            await _publishEndpoint.Publish<OrderPlacedEvent>(new OrderPlacedEvent(
                OrderId: Guid.NewGuid(),
                UserId: request.UsuarioId,
                JogosIds: request.JogosIds,
                PrecoTotal: precoTotal), cancellationToken);
            return true;
        }
    }
}
