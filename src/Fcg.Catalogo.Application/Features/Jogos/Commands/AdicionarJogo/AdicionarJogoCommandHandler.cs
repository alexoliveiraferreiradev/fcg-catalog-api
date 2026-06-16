using Fcg.Catalogo.Application.Dtos.Jogos;
using Fcg.Catalogo.Domain.Common.Exceptions;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Domain.Resources;
using Fcg.Catalogo.Domain.ValueObject;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Jogos.Commands.AdicionarJogo
{
    public class AdicionarJogoCommandHandler : IRequestHandler<AdicionarJogoCommand, JogosResponse>
    {
        private readonly IJogoRepository _jogoRepository;
        public AdicionarJogoCommandHandler(IJogoRepository jogoRepository)
        {
            _jogoRepository = jogoRepository;
        }
        public async Task<JogosResponse> Handle(AdicionarJogoCommand request, CancellationToken cancellationToken)
        {
            var nomeJaExistente = await VerificaDuplicidadeNome(request.Nome);
            if(nomeJaExistente)
            {
                throw new DomainException(MensagensDominio.JogoMesmoNomeExistente);
            }
            var preco = new Preco(request.Preco);
            var nomeJogo = new Nome(request.Nome);
            var descricaoJogo = new Descricao(request.Descricao);
            var jogo = new Jogo(nomeJogo, descricaoJogo, preco, request.Genero);
            await _jogoRepository.Adicionar(jogo);
           
            return new JogosResponse
            {
                Id = jogo.Id,
                Nome = jogo.Nome.Valor,
                Descricao = jogo.Descricao.Valor,
                PrecoOriginal = jogo.PrecoBase.Valor,
                Genero = jogo.Genero
            };
        }

        public async Task<bool> VerificaDuplicidadeNome(string nomeJogo)
        {
            return await _jogoRepository.ExisteJogoComNome(nomeJogo);
        }
    }
}
