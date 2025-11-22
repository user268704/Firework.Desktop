using Firework.Server.Filters;

namespace Firework.Server.Endpoints;

public class ClientAuthorizationEndpointFilter : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context, 
        EndpointFilterDelegate next)
    {
        if (!ClientAuthorizationFilter.TryGetClient(context.HttpContext, out var client))
        {
            return Results.Unauthorized();
        }

        // Сохраняем клиента в HttpContext.Items для доступа в эндпоинтах
        context.HttpContext.Items["AuthorizedClient"] = client;

        return await next(context);
    }
}