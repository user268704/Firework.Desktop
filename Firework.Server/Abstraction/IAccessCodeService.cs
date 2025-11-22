using FluentResults;

namespace Firework.Server.Abstraction;

public interface IAccessCodeService
{
    Result EnsureValid(string submittedCode);
    Task<Result<string>> RotateAsync(CancellationToken cancellationToken = default);
    string CurrentCode { get; }
}

