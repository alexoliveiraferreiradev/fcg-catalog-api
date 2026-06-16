using MediatR;

namespace Fcg.Catalogo.Application.Features.Jogos.Queries.VerificaDuplicidadeDoNome
{
    public record VerificaDuplicidadeDoNomeQuery(string NomeJogo) : IRequest<bool>;
}
