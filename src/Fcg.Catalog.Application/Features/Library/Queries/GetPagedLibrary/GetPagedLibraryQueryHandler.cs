using Dapper;
using Fcg.Catalog.Application.Common.Interfaces;
using Fcg.Core.Abstractions.Common;
using MediatR;
using System.Data;

namespace Fcg.Catalog.Application.Features.Library.Queries.GetPagedLibrary
{
    public class GetPagedLibraryQueryHandler : IRequestHandler<GetPagedLibraryQuery, PagedResult<BibliotecaItemResponse>>
    {
        
        private readonly ICacheService _cacheService;
        private readonly ILibraryQueryRepository _libraryQueryRepository;
        public GetPagedLibraryQueryHandler(ILibraryQueryRepository libraryQueryRepository, ICacheService cacheService)
        {
            _libraryQueryRepository = libraryQueryRepository;
            _cacheService = cacheService;
        }
        public async Task<PagedResult<BibliotecaItemResponse>> Handle(GetPagedLibraryQuery request, CancellationToken cancellationToken)
        {
            var cachaKey= $"library:u_{request.UserId}:p{request.Page}:t{request.PageSize}";
            var bibliotecaEmCache = await _cacheService.GetAsync<PagedResult<BibliotecaItemResponse>>(cachaKey, cancellationToken);
            if (bibliotecaEmCache != null)
            {
                return bibliotecaEmCache;
            }

            var bibliotecaPaginada = await _libraryQueryRepository.GetPagedLibraryAsync(request.UserId, request.Page, request.PageSize, cancellationToken); 

            if (bibliotecaPaginada.Items.Any())
            {
                await _cacheService.SetAsync(cachaKey, bibliotecaPaginada, TimeSpan.FromMinutes(5), cancellationToken);
            }

            return bibliotecaPaginada;
        }
    }
}
