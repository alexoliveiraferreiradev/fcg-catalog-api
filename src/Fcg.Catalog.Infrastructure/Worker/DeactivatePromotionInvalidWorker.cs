using Fcg.Catalog.Domain.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Fcg.Catalog.Infrastructure.Worker
{
    public class DeactivateInvalidPromotionWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public DeactivateInvalidPromotionWorker(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var GameRepository = scope.ServiceProvider.GetRequiredService<IGameRepository>();
                    await GameRepository.DeactivateInvalidPromotions();
                }
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
        }
    }
}
