using MediatR;

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
        public Guid GameId { get; init; }
    }
}
