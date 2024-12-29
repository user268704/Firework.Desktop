using System.Reflection;

namespace Firework.Abstraction.Services.FileService;

public interface IFileService
{
    public void WriteString(string filePath, string text);
    public void WriteBytes(string filePath, byte[] bytes, bool createIfNotExists = true);
    public void ReadString(string filePath);
    public void ReadBytes(string filePath);
}