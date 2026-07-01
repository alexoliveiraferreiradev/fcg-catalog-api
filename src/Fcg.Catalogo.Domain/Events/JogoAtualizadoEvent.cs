using MediatR;

namespace Fcg.Catalogo.Domain.Events;

public class JogoAtualizadoEvent : INotification
{
    public Guid JogoId { get; }

    public JogoAtualizadoEvent(Guid jogoId)
    {
        JogoId = jogoId;
    }
}
