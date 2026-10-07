using Prometheus;

namespace Fcg.Catalog.Application.Extensions.Observability
{
    internal static class PromotionMetrics
    {
        internal static readonly Counter CreateAttempts = Metrics.CreateCounter(
            "promotion_create_attempts_total",
            "Total de tentativas de criação de promoções",
            new CounterConfiguration
            {
                LabelNames = new[] { "result" }
            });

        internal static readonly Counter UpdateAttempts = Metrics.CreateCounter(
            "promotion_update_attempts_total",
            "Total de tentativas de atualização de promoções",
            new CounterConfiguration
            {
                LabelNames = new[] { "result" }
            });

        internal static readonly Counter DeactivateAttempts = Metrics.CreateCounter(
            "promotion_deactivate_attempts_total",
            "Total de tentativas de desativação de promoções",
            new CounterConfiguration
            {
                LabelNames = new[] { "result" }
            });
    }
}
