using System.Net;

namespace Firework.Models.Server;

public class HttpResult
{
    public HttpStatusCode StatusCode { get; set; }
    public string Data { get; set; }
}