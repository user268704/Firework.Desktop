using Firework.Server.Models.Clients;

namespace Firework.Server.Models.Commands;

public sealed class CommandContext
{
    public CommandContext(RegisteredClient client)
    {
        Client = client;
    }

    public RegisteredClient Client { get; }
}