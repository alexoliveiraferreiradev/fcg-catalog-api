namespace Fcg.Catalogo.Application.MessageContracts
{
    public record UserCreatedEvent(
         Guid UserId,
         string Nome,
         string Email);
}
