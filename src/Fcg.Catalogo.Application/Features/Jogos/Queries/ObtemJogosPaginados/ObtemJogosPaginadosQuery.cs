using Fcg.Catalogo.Application.Common;
using Fcg.Catalogo.Application.Dtos.Jogos;
using Fcg.Catalogo.Domain.Enum;
using MediatR;

namespace Fcg.Catalogo.Application.Features.Jogos.Queries.ObtemJogosPaginados
{
    public record ObtemJogosPaginadosQuery(
        int Pagina = 1,
        int Tamanho = 10,
        string? Busca = null,
        GeneroJogo? Genero = null,
        bool? ApenasPromovidos = null) : IRequest<PagedResult<JogosResponse>>;
}
