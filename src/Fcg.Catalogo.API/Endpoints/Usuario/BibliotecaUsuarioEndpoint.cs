
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
            var group = app.MapGroup("/api/usuario/biblioteca").WithTags("Biblioteca do Usuário");

            group.MapGet("/obtem-todos",
                async ([FromServices] ISender mediator,
                       ClaimsPrincipal user,
                       [FromQuery] int pagina = 1,
                       [FromQuery] int tamanho = 10) =>
            {
               var curretUserId = Guid.Parse("aea0b4f3-d220-4c8d-aba8-d868be7ca593");

                var query = new ObtemBibliotecaPaginadaQuery(curretUserId, pagina, tamanho);

                var response = mediator.Send(query);

                if (response == null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(response);
            });
        }
    }
}
