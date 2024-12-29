using Firework.Abstraction.Services.FileService;

namespace Firework.Core.Services;

public class FileService : IFileService
{
    public void WriteString(string filePath, string text)
    {
        throw new NotImplementedException();
    }

    public void WriteBytes(string filePath, byte[] bytes, bool createIfNotExists = true)
    {
        if (!Path.Exists(filePath) && !createIfNotExists)
            return;
        
        if (!Path.Exists(filePath) && createIfNotExists) 
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        
        using var fileStream = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write);
        
        fileStream.Write(bytes, 0, bytes.Length);
    }

    public void ReadString(string filePath)
    {
        throw new NotImplementedException();
    }

    public void ReadBytes(string filePath)
    {
        throw new NotImplementedException();
    }
}