using Firework.Models.RequestContext;

namespace Firework.Server.Services;

public class RequestContextService
{
    public RequestContext CreateContextFromHttpContext(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        var userAgent = context.Request.Headers["User-Agent"].ToString() ?? "Unknown";

        return new RequestContext
        {
            IP = ip,
            UserAgent = userAgent,
            DateRequest = DateTime.UtcNow
        };
    }
}