using System.IO.Pipes;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Firework.Core.Services.PipeBroker;

public class PipeService : IDisposable
{
    internal string PipeName { get; set; }
    internal Action<byte[]> OnReceiveMessage { get; set; }
    internal Action OnConnected { get; set; }

    internal bool IsEnableLogging { get; set; }
    internal ILogger Logger { get; set; }
    internal bool IsClient { get; set; }

    private NamedPipeClientStream _pipeClient;
    
    private void StartClient()
    {
        try
        {   
            _pipeClient = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut);
            
            if (IsEnableLogging)
            {
                Logger.LogInformation("Подключение к серверу pipe: {PipeName}", PipeName);
            }
            
            _pipeClient.Connect(5000); // Таймаут 5 секунд
            OnConnected?.Invoke();

            if (IsEnableLogging)
            {
                Logger.LogInformation("Подключено к серверу pipe");
            }
            
            // Читаем сообщения в отдельном потоке
            Task.Run(ReadMessages);
        }
        catch (Exception ex)
        {
            if (IsEnableLogging)
            {
                Logger.LogError(ex, "Ошибка подключения к pipe серверу");
            }
            throw;
        }
    }
    
    private void ReadMessages()
    {
        try
        {
            byte[] buffer = new byte[1024];
            
            while (_pipeClient.IsConnected)
            {
                int bytesRead = _pipeClient.Read(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    OnReceiveMessage?.Invoke(buffer);

                    if (IsEnableLogging)
                    {
                        Logger.LogInformation("Получено сообщение, байт: {BytesCount}", bytesRead);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            if (IsEnableLogging)
            {
                Logger.LogError(ex, "Ошибка при чтении сообщений из pipe");
            }
        }
    }

    public void SendMessage(byte[] data)
    {
        try
        {
            if (_pipeClient?.IsConnected == true)
            {
                _pipeClient.Write(data, 0, data.Length);
                _pipeClient.Flush();
                
                if (IsEnableLogging)
                {
                    Logger.LogInformation("Отправлено сообщение, байт: {BytesCount}", data.Length);
                }
            }
            else
            {
                if (IsEnableLogging)
                {
                    Logger.LogWarning("Попытка отправки сообщения при отсутствии подключения");
                }
            }
        }
        catch (Exception ex)
        {
            if (IsEnableLogging)
            {
                Logger.LogError(ex, "Ошибка при отправке сообщения");
            }
            throw;
        }
    }
    
    public void SendMessage(string message)
    {
        SendMessage(Encoding.UTF8.GetBytes(message));
    }
    
    private void StartServer()
    {
        using var pipeServer = new NamedPipeServerStream(PipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Byte);
        
        pipeServer.WaitForConnection();
        OnConnected?.Invoke();

        if (IsEnableLogging)
        {
            Logger.LogInformation("Client connected to pipe");
        }
            
        byte[] buffer = new byte[pipeServer.InBufferSize];
            
        while (pipeServer.Read(buffer, 0, buffer.Length) != -1)
        {
            OnReceiveMessage?.Invoke(buffer);

            if (IsEnableLogging)
            {
                Logger.LogInformation("Received message");
            }
        }
        
    }
    
    
    public void Start()
    {
        if (IsClient)
        {
            StartClient();
        }
        else
        {
            StartServer();
        }
    }
    
    public void Dispose()
    {
        try
        {
            _pipeClient?.Close();
            _pipeClient?.Dispose();
        }
        catch (Exception ex)
        {
            if (IsEnableLogging)
            {
                Logger?.LogError(ex, "Ошибка при освобождении ресурсов PipeService");
            }
        }
    }
}