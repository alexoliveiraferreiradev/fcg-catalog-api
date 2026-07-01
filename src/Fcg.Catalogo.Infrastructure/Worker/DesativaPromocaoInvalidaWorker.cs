using Fcg.Catalogo.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fcg.Catalogo.Infrastructure.Worker
{
    public class DesativaPromocaoInvalidaWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public DesativaPromocaoInvalidaWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var jogoRepository = scope.ServiceProvider.GetRequiredService<IJogoRepository>();
                    await jogoRepository.DesativaPromocoesInvalidas();
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
