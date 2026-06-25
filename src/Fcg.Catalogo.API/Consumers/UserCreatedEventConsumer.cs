using Fcg.Catalogo.Domain.Entities;
using Fcg.Catalogo.Domain.Repositories;
using Fcg.Core.Abstractions.Interfaces;
using Fcg.Core.Abstractions.MessageContracts;
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

            _logger.LogInformation("[CatalogAPI] Preparando Biblioteca para o novo usuário: {Nome}", mensagem.Name);

            var bibliotecaExistente = await _bibliotecaRepository.ObterPorId(mensagem.UserId);

            if(bibliotecaExistente != null)
            {
                _logger.LogInformation("[CatalogAPI] Biblioteca já existe para o usuário: {Nome}", mensagem.Name);
                return;
            }
            var biblioteca = new Biblioteca(mensagem.UserId);

            _bibliotecaRepository.Adicionar(biblioteca);

            await _unitOfWork.CommitAsync();

            _logger.LogInformation("[CatalogAPI] Biblioteca criada para o usuário: {Nome}", mensagem.Name);
        }
    }
}
