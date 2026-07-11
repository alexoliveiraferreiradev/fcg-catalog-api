namespace Fcg.Catalog.Infrastructure.MessageBroker
{
    public class RabbitMqQueueOptions
    {
        public const string SectionName = "RabbitMqQueues";
        public string CatalogPaymentFailedQueue { get; set; } = string.Empty;
        public string CatalogPaymentProcessedQueue { get; set; } = string.Empty;
    }
}
