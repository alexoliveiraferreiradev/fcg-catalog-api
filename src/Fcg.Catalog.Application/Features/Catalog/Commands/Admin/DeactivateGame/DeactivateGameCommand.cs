using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateGame
{
    /// <summary>
    /// Comando para desativar logicamente um game do catálogo.
    /// </summary>
    /// <param name="GameId">Identificador único (GUID) do game a ser desativado.</param>
    public record DeactivateGameCommand(Guid GameId) : IRequest;
}
