using Prometheus;

namespace Fcg.Catalog.Application.Extensions.Observability
{
    internal static class GameMetrics
    {
        internal static readonly Counter CreateAttempts = Metrics.CreateCounter(
        "game_create_attempts_total", "Total de tentativas de criação de jogos",
        new CounterConfiguration { LabelNames = new[] { "result" } });

        internal static readonly Counter UpdateAttempts = Metrics.CreateCounter(
            "game_update_attempts_total", "Total de tentativas de atualização de jogos",
            new CounterConfiguration { LabelNames = new[] { "result" } });

        internal static readonly Counter ActivateAttempts = Metrics.CreateCounter(
            "game_activate_attempts_total", "Total de tentativas de ativação de jogos",
            new CounterConfiguration { LabelNames = new[] { "result" } });

        internal static readonly Counter DeactivateAttempts = Metrics.CreateCounter(
            "game_deactivate_attempts_total", "Total de tentativas de desativação de jogos",
            new CounterConfiguration { LabelNames = new[] { "result" } });

        internal static readonly Counter RepublishAttempts = Metrics.CreateCounter(
            "game_republish_attempts_total", "Total de tentativas de republicação de jogos",
            new CounterConfiguration { LabelNames = new[] { "result" } });
    }
}
