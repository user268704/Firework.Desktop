namespace Firework.Core.Crypto;

public class DeviceHashService
{
    public static string GenerateHash(string deviceName, string operatingSystem, string deviceSignature)
    {
        var rawHash = $"{deviceName}-{operatingSystem}-{deviceSignature}";
        
        using var sha256 = System.Security.Cryptography.SHA256.Create();
        
        var inputBytes = System.Text.Encoding.UTF8.GetBytes(rawHash);
        
        var bytes = sha256.ComputeHash(inputBytes);
        
        return Convert.ToBase64String(bytes);
    }
}