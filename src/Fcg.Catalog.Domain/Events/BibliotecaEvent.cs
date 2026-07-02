using MediatR;

namespace Fcg.Catalog.Domain.Events
{
    public class BibliotecaEvent : INotification
    {
        public Guid UsuarioId { get; set; }

        public BibliotecaEvent(Guid UsuarioId)
        {
            this.UsuarioId = UsuarioId; 
        }
    }
}
