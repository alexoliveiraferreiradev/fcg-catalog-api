using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class JogoReativadoEvent : INotification
{
    public Guid JogoId { get; }

    public JogoReativadoEvent(Guid jogoId)
    {
        JogoId = jogoId;
    }
}
