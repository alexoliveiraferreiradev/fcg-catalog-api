namespace Fcg.Catalog.Application.Common.Interfaces
{
    public interface IIntegrationEventPublisher
    {
        Task PublishAsync<T>(object integrationEvent, CancellationToken cancellationToken = default) where T : class;
    }
}
