using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.CheckNameDuplicity
{
    public record VerificaDuplicidadeDoNomeQuery(string GameName) : IRequest<bool>;
}
