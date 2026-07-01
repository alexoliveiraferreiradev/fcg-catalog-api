using MediatR;

namespace Fcg.Catalogo.Domain.Events;

public class PromocaoAtualizadaEvent : INotification
{
    public Guid JogoId { get; }
    public Guid PromocaoId { get; }

    public PromocaoAtualizadaEvent(Guid jogoId, Guid promocaoId)
    {
        JogoId = jogoId;
        PromocaoId = promocaoId;
    }
}
