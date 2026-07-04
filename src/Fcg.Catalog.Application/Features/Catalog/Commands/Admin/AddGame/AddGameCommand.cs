using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using MediatR;
using System.ComponentModel;

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
        [DefaultValue("nome do jogo")]
        public string Name { get; init; } = string.Empty;
        /// <summary>
        /// Descrição detalhada do jogo.
        /// </summary>
        [DefaultValue("descrição do jogo")]
        public string Description { get; init; } = string.Empty;
        /// <summary>
        /// Preço base do jogo.
        /// </summary>
        [DefaultValue(59.99)]
        public decimal Price { get; init; }
        /// <summary>
        /// Gênero do jogo.
        /// </summary>
        [DefaultValue(GameGenre.Action)]
        public GameGenre Genre { get; init; }
    }
}
