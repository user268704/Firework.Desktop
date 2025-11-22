using System.Linq;
using Firework.Server.Abstraction;
using Firework.Server.Models.Clients;
using FluentResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Firework.Server.Filters;

public sealed class ClientAuthorizationFilter : IAsyncActionFilter
{
    private const string CLIENT_CONTEXT_KEY = "AuthenticatedClient";
    private readonly IClientRegistry _clientRegistry;

    public ClientAuthorizationFilter(IClientRegistry clientRegistry, ILogger<ClientAuthorizationFilter> logger)
    {
        _clientRegistry = clientRegistry;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var tokenResult = ExtractToken(context.HttpContext.Request);

        if (tokenResult.IsFailed)
        {
            context.Result = new UnauthorizedObjectResult(tokenResult.Errors.First().Message);
            return;
        }

        var clientResult = _clientRegistry.GetByToken(tokenResult.Value);

        if (clientResult.IsFailed)
        {
            context.Result = new UnauthorizedObjectResult(clientResult.Errors.First().Message);
            return;
        }

        context.HttpContext.Items[CLIENT_CONTEXT_KEY] = clientResult.Value;
        await next();
    }

    private static Result<string> ExtractToken(HttpRequest request)
    {
        if (!request.Headers.TryGetValue("Authorization", out var values))
        {
            return Result.Fail("Authorization header is missing.");
        }

        var headerValue = values.FirstOrDefault();

        if (string.IsNullOrWhiteSpace(headerValue))
        {
            return Result.Fail("Authorization header is empty.");
        }

        const string prefix = "Bearer ";

        if (!headerValue.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            return Result.Fail("Authorization header must use Bearer scheme.");
        }

        var token = headerValue[prefix.Length..].Trim();
        return string.IsNullOrWhiteSpace(token)
            ? Result.Fail("Token is missing.")
            : Result.Ok(token);
    }

    public static bool TryGetClient(HttpContext context, out RegisteredClient client)
    {
        if (context.Items.TryGetValue(CLIENT_CONTEXT_KEY, out var value) && value is RegisteredClient registeredClient)
        {
            client = registeredClient;
            return true;
        }

        client = null!;
        return false;
    }
}

