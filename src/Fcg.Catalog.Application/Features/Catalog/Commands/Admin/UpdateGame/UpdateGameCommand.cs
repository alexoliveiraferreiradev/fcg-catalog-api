using Fcg.Catalog.Application.Features.Response;
using Fcg.Catalog.Domain.Enum;
using MediatR;
using System.ComponentModel.DataAnnotations;

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
        public Guid GameId { get; init; }
        /// <summary>
        /// Novo nome/título para o game.
        /// </summary>
        public string NovoNome { get; init; }
        /// <summary>
        /// Nova descrição detalhada do game.
        /// </summary>
        public string NovaDescricao { get; init; }
        /// <summary>
        /// Novo preço base do game.
        /// </summary>
        public decimal NovoPreco { get; init; }
        /// <summary>
        /// Novo gênero principal do game.
        /// </summary>
        public GameGenre NovoGenero { get; init; }

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
            NovoNome = novoNome; NovaDescricao = novaDescricao; NovoPreco = novoPreco; NovoGenero = novoGenero;
        }
    }
}
