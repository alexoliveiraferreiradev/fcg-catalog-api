using MediatR;

namespace Fcg.Catalog.Domain.Events;

public class JogoAtualizadoEvent : INotification
{
    public Guid JogoId { get; }

    public JogoAtualizadoEvent(Guid jogoId)
    {
        JogoId = jogoId;
    }
}
