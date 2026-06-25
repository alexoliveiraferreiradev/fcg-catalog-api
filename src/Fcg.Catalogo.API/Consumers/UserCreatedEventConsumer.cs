using Fcg.Catalogo.Application.MessageContracts;
using Fcg.Catalogo.Domain.Common.Interfaces;
using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Repositories;
using MassTransit;

namespace Fcg.Catalogo.API.Consumers
{
    public class UserCreatedEventConsumer : IConsumer<UserCreatedEvent>
    {
        private readonly IBibliotecaRepository _bibliotecaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserCreatedEventConsumer> _logger;
        public UserCreatedEventConsumer(IBibliotecaRepository bibliotecaRepository, IUnitOfWork unitOfWork, 
            ILogger<UserCreatedEventConsumer> logger)
        {
            _bibliotecaRepository = bibliotecaRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<UserCreatedEvent> context)
        {
            var mensagem = context.Message;

            _logger.LogInformation("[CatalogAPI] Preparando Biblioteca para o novo usuário: {Nome}", mensagem.Nome);

            var bibliotecaExistente = await _bibliotecaRepository.ObterPorId(mensagem.UserId);

            if(bibliotecaExistente != null)
            {
                _logger.LogInformation("[CatalogAPI] Biblioteca já existe para o usuário: {Nome}", mensagem.Nome);
                return;
            }
            var biblioteca = new Biblioteca(mensagem.UserId);

            _bibliotecaRepository.Adicionar(biblioteca);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[CatalogAPI] Biblioteca criada para o usuário: {Nome}", mensagem.Nome);
        }
    }
}
