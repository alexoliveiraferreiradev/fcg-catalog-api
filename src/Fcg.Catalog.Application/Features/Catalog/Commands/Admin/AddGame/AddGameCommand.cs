using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.AddGame
{
    /// <summary>
    /// Comando para adicionar um novo game ao catálogo.
    /// </summary>
    public record AddGameCommand : IRequest<GameResponse>
    {
        /// <summary>
        /// Nome/Título do jogo.
        /// </summary>
        public string Name { get; init; } = string.Empty;
        /// <summary>
        /// Descrição detalhada do jogo.
        /// </summary>
        public string Description { get; init; } = string.Empty;
        /// <summary>
        /// Preço base do jogo.
        /// </summary>
        public decimal Price { get; init; }
        /// <summary>
        /// Gênero do jogo.
        /// </summary>
        public GameGenre Genre { get; init; }
    }
}
