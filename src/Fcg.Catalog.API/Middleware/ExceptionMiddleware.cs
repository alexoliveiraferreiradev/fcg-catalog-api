using Fcg.Core.Abstractions.Common.Exceptions;
using System.Net;

namespace Fcg.Catalog.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var statusCode = (int)HttpStatusCode.InternalServerError;
            var message = "Ocorreu um erro interno no servidor. Tente novamente mais tarde.";
            if (exception is DomainException)
            {
                _logger.LogInformation("Regra de Negócio: {Mensagem}", exception.Message);
                statusCode = (int)HttpStatusCode.BadRequest;
                message = exception.Message;
            }
            else
            {
                _logger.LogError(exception, "Erro Crítico: {Mensagem}", exception.Message);
            }


            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsJsonAsync(new { erro = message });
        }
    }
}
