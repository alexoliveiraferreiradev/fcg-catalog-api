using MediatR;
using System.ComponentModel;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.DeactivateGame
{
    /// <summary>
    /// Comando para desativar logicamente um game do catálogo.
    /// </summary>
    /// <param name="GameId">Identificador único (GUID) do game a ser desativado.</param>
    public record DeactivateGameCommand(
        [property: DefaultValue("00000000-0000-0000-0000-000000000000")] Guid GameId
    ) : IRequest;
}
