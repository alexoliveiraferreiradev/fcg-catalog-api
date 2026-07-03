using Fcg.Catalog.Application.Features.Response;
using MediatR;

namespace Fcg.Catalog.Application.Features.Orders.Commands.PlaceOrder
{
    /// <summary>
    /// Comando enviado para registrar uma nova intenção de compra de jogos.
    /// </summary>
    /// <param name="UserId">Identificador único (GUID) do usuário que está realizando a compra.</param>
    /// <param name="NomeUsuario">Nome do usuário comprador.</param>
    /// <param name="EmailUsuario">Endereço de e-mail do usuário comprador.</param>
    /// <param name="JogosIds">Lista de identificadores únicos (GUIDs) dos jogos a serem comprados.</param>
    public record PlaceOrderCommand(Guid UserId, string NomeUsuario, string EmailUsuario, IEnumerable<Guid> JogosIds) : IRequest<bool>;
}
