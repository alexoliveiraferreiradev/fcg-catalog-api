using Fcg.Catalogo.Application.Common.Interfaces;
using Fcg.Catalogo.Application.Features.Response;
using Fcg.Catalogo.Domain.Events;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Catalogo.Domain.ValueObject;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.Resources;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fcg.Catalogo.Application.Features.Catalogo.Commands.Admin.AtualizarJogo
{
    public class AtualizarJogoCommandHandler : IRequestHandler<AtualizarJogoCommand, JogoResponse>
    {
        private readonly IJogoRepository _jogoRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AtualizarJogoCommandHandler> _logger;
        private readonly IMediator _mediator;

        public AtualizarJogoCommandHandler(
            IJogoRepository jogoRepository,
            IUnitOfWork unitOfWork,
            ILogger<AtualizarJogoCommandHandler> logger,
            IMediator mediator)
        {
            _jogoRepository = jogoRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<JogoResponse> Handle(AtualizarJogoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[CatalogAPI] Iniciando processo para atualizar jogo. ID: {JogoId}, NovoNome: {NovoNome}", request.JogoId, request.NovoNome);

            var jogo = await _jogoRepository.ObterPorId(request.JogoId);
            if (jogo == null)
            {
                _logger.LogWarning("[CatalogAPI] Falha ao atualizar jogo. Jogo não encontrado. ID: {JogoId}", request.JogoId);
                throw new DomainException(MensagensDominio.JogoNaoEncontrado);
            }

            var novoNome = new Nome(request.NovoNome);
            var novaDescricao = new Descricao(request.NovaDescricao);
            var novoPreco = new Preco(request.NovoPreco);

            jogo.Atualizar(novoNome, novaDescricao, novoPreco, request.NovoGenero);
            _jogoRepository.Atualizar(jogo);
            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[CatalogAPI] Jogo atualizado com sucesso. ID: {JogoId}", jogo.Id);

            await _mediator.Publish(new JogoAtualizadoEvent(request.JogoId), cancellationToken);

            return new JogoResponse
            {
                Id = jogo.Id,
                Nome = jogo.Nome.Valor,
                Descricao = jogo.Descricao.Valor,
                PrecoOriginal = jogo.PrecoBase.Valor,
                Genero = jogo.Genero,
                Ativo = jogo.Ativo
            };
        }
    }
}
