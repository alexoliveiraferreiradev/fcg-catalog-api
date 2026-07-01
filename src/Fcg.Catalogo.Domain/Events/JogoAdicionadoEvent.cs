using MediatR;

namespace Fcg.Catalogo.Domain.Events
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
