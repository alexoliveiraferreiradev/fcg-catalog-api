
using Fcg.Catalogo.Application.Features.Biblioteca.Queries.ObtemBibliotecaPaginada;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Fcg.Catalogo.API.Endpoints.Usuario
{
    public static class BibliotecaUsuarioEndpoint
    {
        public static void MapBibliotecaUsuarioPaginadaEndpoint(this WebApplication app)
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
