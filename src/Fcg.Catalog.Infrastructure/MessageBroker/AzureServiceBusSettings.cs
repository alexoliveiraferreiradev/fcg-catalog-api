using System.ComponentModel.DataAnnotations;

namespace Fcg.Catalog.Infrastructure.MessageBroker
{
    internal class AzureServiceBusSettings
    {
        public const string SectionName = "AzureServiceBus";
        [Required]
        public string ConnectionString { get; set; } = string.Empty;
        [Required(ErrorMessage = "CatalogGameRatedIntegrationQueue é obrigatório")]
        public string CatalogGameRatedIntegrationQueue { get; set; } = string.Empty;
    }
}
