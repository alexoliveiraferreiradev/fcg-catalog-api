using MediatR;

namespace Fcg.Catalogo.Domain.Events;

public class JogoDesativadoEvent : INotification
{
    public Guid JogoId { get; }

    public JogoDesativadoEvent(Guid jogoId)
    {
        JogoId = jogoId;
    }
}
