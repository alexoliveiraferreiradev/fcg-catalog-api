using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class PromocaoDesativadaEvent : INotification
{
    public Guid JogoId { get; }
    public Guid PromocaoId { get; }

    public PromocaoDesativadaEvent(Guid jogoId, Guid promocaoId)
    {
        JogoId = jogoId;
        PromocaoId = promocaoId;
    }
}
