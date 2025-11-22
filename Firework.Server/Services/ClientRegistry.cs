using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Firework.Server.Abstraction;
using Firework.Server.Dto.Devices;
using Firework.Server.Models.Clients;
using FluentResults;

namespace Firework.Server.Services;

public sealed class ClientRegistry : IClientRegistry
{
    private readonly ConcurrentDictionary<string, RegisteredClient> _clientsByToken = new();
    private readonly ConcurrentDictionary<Guid, string> _tokenByDevice = new();

    public Result<RegisteredClient> Register(DevicePayloadDto payload, string ipAddress)
    {
        if (_tokenByDevice.TryGetValue(payload.DeviceId, out var existingToken))
        {
            if (_clientsByToken.TryGetValue(existingToken, out var existingClient))
            {
                existingClient.LastSeenUtc = DateTime.UtcNow;
                existingClient.Ip = ipAddress;
                return Result.Ok(existingClient);
            }
        }

        var token = GenerateToken();

        var client = new RegisteredClient
        {
            DeviceId = payload.DeviceId,
            DeviceName = payload.DeviceName,
            Ip = ipAddress,
            Token = token,
            RegisteredAtUtc = DateTime.UtcNow,
            LastSeenUtc = DateTime.UtcNow
        };

        if (!_clientsByToken.TryAdd(token, client))
        {
            return Result.Fail("Unable to register client.");
        }

        _tokenByDevice[payload.DeviceId] = token;
        return Result.Ok(client);
    }

    public Result<RegisteredClient> GetByToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return Result.Fail("Token is required.");
        }

        return _clientsByToken.TryGetValue(token, out var client)
            ? Result.Ok(client)
            : Result.Fail("Client not found.");
    }

    public Result<IReadOnlyCollection<RegisteredClient>> GetAll()
    {
        return Result.Ok<IReadOnlyCollection<RegisteredClient>>(_clientsByToken.Values.ToList());
    }

    public void Touch(string token)
    {
        if (_clientsByToken.TryGetValue(token, out var client))
        {
            client.LastSeenUtc = DateTime.UtcNow;
        }
    }

    private static string GenerateToken()
    {
        Span<byte> buffer = stackalloc byte[32];
        RandomNumberGenerator.Fill(buffer);
        return Convert.ToBase64String(buffer);
    }
}

