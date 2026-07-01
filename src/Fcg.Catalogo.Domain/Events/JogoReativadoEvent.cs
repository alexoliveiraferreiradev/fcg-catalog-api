using MediatR;

namespace Fcg.Catalogo.Domain.Events;

public class JogoReativadoEvent : INotification
{
    public Guid JogoId { get; }

    public JogoReativadoEvent(Guid jogoId)
    {
        JogoId = jogoId;
    }
}
