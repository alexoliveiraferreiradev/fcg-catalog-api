using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class JogoDesativadoEvent : INotification
{
    public Guid JogoId { get; }

    public JogoDesativadoEvent(Guid jogoId)
    {
        JogoId = jogoId;
    }
}
