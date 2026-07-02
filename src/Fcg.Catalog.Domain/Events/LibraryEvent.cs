using MediatR;

namespace Fcg.Catalog.Domain.Events
{
    public class LibraryEvent : INotification
    {
        public Guid UserId { get; set; }

        public LibraryEvent(Guid userId)
        {
            UserId = userId; 
        }
    }
}
