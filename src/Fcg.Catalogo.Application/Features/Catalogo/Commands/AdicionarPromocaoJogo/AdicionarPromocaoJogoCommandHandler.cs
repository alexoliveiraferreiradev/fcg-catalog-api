using Fcg.Catalogo.Application.Features.Response;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Domain.Resources;
using Fcg.Catalogo.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.AdicionarPromocaoJogo
{
    public class AdicionarPromocaoJogoCommandHandler : IRequestHandler<AdicionarPromocaoJogoCommand, PromocaoResponse>
    {
        private readonly IJogoRepository _jogoRepository;
        public AdicionarPromocaoJogoCommandHandler(IJogoRepository jogoRepository)
        {
            _jogoRepository = jogoRepository;   
        }
        public async Task<PromocaoResponse> Handle(AdicionarPromocaoJogoCommand request, CancellationToken cancellationToken)
        {
            var periodo = new Periodo(request.DataInicio, request.DataFim);
            var jogo = await _jogoRepository.ObterPorId(request.JogoId);
            if (jogo == null) throw new DomainException(MensagensDominio.JogoNaoEncontrado);

            if (jogo.Promocoes.Any()) throw new DomainException(MensagensDominio.JogoPromocoes);

            var valorPromocao = new Preco(request.ValorPromocao);   
            jogo.AdicionarPromocao(valorPromocao, periodo);
            _jogoRepository.Atualizar(jogo);

            var novaPromocao = jogo.Promocoes.First();

            return new PromocaoResponse
            {
                JogoId = jogo.Id,
                DescricaoJogo = jogo.Descricao.Valor,
                NomeJogo = jogo.Nome.Valor,
                PromocaoId = novaPromocao.Id,
                ValorPromocao = novaPromocao.ValorPromocao.Valor,
                DataFim = novaPromocao.Periodo.DataFim,
                DataInicio = novaPromocao.Periodo.DataInicio
            };
                
        }
    }
}
