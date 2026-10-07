using FluentValidation;

namespace Fcg.Catalog.API.Filters
{
    public class ValidationFilter<T> : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var argument = context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(T));

            if (argument == null)
            {
                return await next(context);
            }

            var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();

            if (validator is not null)
            {
                var validationResult = await validator.ValidateAsync((T)argument);

                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }
            }

            return await next(context);
        }
    }
}
