using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using MediatR;
using System.ComponentModel;

namespace Fcg.Catalog.Application.Features.Catalog.Commands.Admin.UpdateGame
{
    /// <summary>
    /// Comando para atualizar os dados cadastrais de um game.
    /// </summary>
    public record UpdateGameCommand : IRequest<GameResponse>
    {
        /// <summary>
        /// Identificador único (GUID) do game a ser atualizado.
        /// </summary>
        [DefaultValue("00000000-0000-0000-0000-000000000000")]
        public Guid GameId { get; init; }
        /// <summary>
        /// Novo nome/título para o game.
        /// </summary>
        [DefaultValue("novo nome do jogo")]
        public string NewName { get; init; }
        /// <summary>
        /// Nova descrição detalhada do game.
        /// </summary>
        [DefaultValue("nova descrição do jogo")]
        public string NewDescription { get; init; }
        /// <summary>
        /// Novo preço base do game.
        /// </summary>
        [DefaultValue(0.00)]
        public decimal NewPrice { get; init; }
        /// <summary>
        /// Novo gênero principal do game.
        /// </summary>
        [DefaultValue(GameGenre.Action)]
        public GameGenre NewGenre { get; init; }

        /// <summary>
        /// Construtor padrão.
        /// </summary>
        public UpdateGameCommand()
        {
        }

        /// <summary>
        /// Construtor parametrizado.
        /// </summary>
        /// <param name="novoNome">Novo nome do game.</param>
        /// <param name="novaDescricao">Nova descrição do game.</param>
        /// <param name="novoPreco">Novo preço do game.</param>
        /// <param name="novoGenero">Novo gênero do game.</param>
        public UpdateGameCommand(string novoNome, string novaDescricao, decimal novoPreco, GameGenre novoGenero)
        {
            NewName = novoNome; NewDescription = novaDescricao; NewPrice = novoPreco; NewGenre = novoGenero;
        }
    }
}
