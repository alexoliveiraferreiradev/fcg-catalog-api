using MediatR;

namespace Fcg.Catalog.Domain.Events
{
    public class JogoAdicionadoEvent : INotification
    {
        public Guid JogoId { get; }

        public JogoAdicionadoEvent(Guid jogoId)
        {
            JogoId = jogoId;
        }
    }
}
