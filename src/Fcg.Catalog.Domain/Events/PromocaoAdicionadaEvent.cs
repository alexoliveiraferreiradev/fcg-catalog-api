using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class PromocaoAdicionadaEvent : INotification
{
    public Guid JogoId { get; }
    public Guid PromocaoId { get; }

    public PromocaoAdicionadaEvent(Guid jogoId, Guid promocaoId)
    {
        JogoId = jogoId;
        PromocaoId = promocaoId;
    }
}
