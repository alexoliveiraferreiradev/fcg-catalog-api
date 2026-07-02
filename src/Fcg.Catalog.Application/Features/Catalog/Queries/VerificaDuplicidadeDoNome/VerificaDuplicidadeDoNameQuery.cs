using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.VerificaDuplicidadeDoNome
{
    public record VerificaDuplicidadeDoNomeQuery(string NomeJogo) : IRequest<bool>;
}
