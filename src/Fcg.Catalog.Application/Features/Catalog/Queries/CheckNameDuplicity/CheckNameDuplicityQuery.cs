using MediatR;

namespace Fcg.Catalog.Application.Features.Catalog.Queries.CheckNameDuplicity
{
    public record CheckNameDuplicityQuery(string GameName) : IRequest<bool>;
}
