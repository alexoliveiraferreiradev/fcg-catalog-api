
using Fcg.Catalog.Application.Features.Library.Queries.GetPagedLibrary;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.Catalog.API.Endpoints.User
{
    public static class LibraryUserEndpoints
    {
        public static void MapLibraryUserEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/usuario/biblioteca").RequireAuthorization("AcessoGeral").WithTags("Biblioteca do Usuário");

            group.MapGet("/obtem-todos",
                async ([FromServices] ISender mediator,
                       ClaimsPrincipal user,
                       [FromQuery] int pagina = 1,
                       [FromQuery] int tamanho = 10) =>
            {
                var currentUserIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(currentUserIdClaim, out var currentUserId))
                {
                    return Results.Unauthorized();
                }

                var query = new ObtemBibliotecaPaginadaQuery(currentUserId, pagina, tamanho);

                var response = await mediator.Send(query);

                if (response == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(response);
            });
        }
    }
}
