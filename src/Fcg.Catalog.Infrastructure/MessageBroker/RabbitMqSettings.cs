namespace Fcg.Catalog.Infrastructure.MessageBroker
{
    public class RabbitMqSettings
    {
        public string Host { get; set; } = string.Empty;
        public ushort Port { get; set; } = 5672;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public RabbitMqSettings(){ }

        public const string SectionName = "RabbitMqSettings";
        public string CatalogPaymentFailedQueue { get; set; } = string.Empty;
        public string CatalogPaymentProcessedQueue { get; set; } = string.Empty;
    }
}
