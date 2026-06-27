using MediatR;

namespace Fcg.Catalogo.Application.Features.Catalogo.Queries.VerificaDuplicidadeDoNome
{
    public record VerificaDuplicidadeDoNomeQuery(string NomeJogo) : IRequest<bool>;
}
