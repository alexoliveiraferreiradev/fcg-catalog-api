using MediatR;
using System.ComponentModel;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.ReactivateGame
{
    /// <summary>
    /// Comando para reativar um game desativado anteriormente no catálogo.
    /// </summary>
    public record ReactivateGameCommand : IRequest
    {
        /// <summary>
        /// Identificador único (GUID) do game a ser reativado.
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid GameId { get; init; }
    }
}
