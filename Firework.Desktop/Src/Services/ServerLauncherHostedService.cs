using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Firework.Desktop.Services;

public class ServerLauncherHostedService : IHostedService
{
	private readonly ILogger<ServerLauncherHostedService> _logger;
	private readonly string _serverPath;
	private readonly string _workingDirectory;
	private static Process? _serverProcess;
	private static IntPtr _jobHandle = IntPtr.Zero;

	// P/Invoke декларации для работы с Job Objects
	[DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
	private static extern IntPtr CreateJobObject(IntPtr a, string lpName);

	[DllImport("kernel32.dll")]
	private static extern bool SetInformationJobObject(IntPtr hJob, JobObjectInfoType infoType,
		IntPtr lpJobObjectInfo, uint cbJobObjectInfoLength);

	[DllImport("kernel32.dll", SetLastError = true)]
	private static extern bool AssignProcessToJobObject(IntPtr job, IntPtr process);

	[DllImport("kernel32.dll", SetLastError = true)]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool CloseHandle(IntPtr hObject);

	private enum JobObjectInfoType
	{
		ExtendedLimitInformation = 9
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct JobObjectBasicLimitInformation
	{
		public long PerProcessUserTimeLimit;
		public long PerJobUserTimeLimit;
		public uint LimitFlags;
		public UIntPtr MinimumWorkingSetSize;
		public UIntPtr MaximumWorkingSetSize;
		public uint ActiveProcessLimit;
		public UIntPtr Affinity;
		public uint PriorityClass;
		public uint SchedulingClass;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct IOCounters
	{
		public ulong ReadOperationCount;
		public ulong WriteOperationCount;
		public ulong OtherOperationCount;
		public ulong ReadTransferCount;
		public ulong WriteTransferCount;
		public ulong OtherTransferCount;
	}

	[StructLayout(LayoutKind.Sequential)]
	private struct JobObjectExtendedLimitInformation
	{
		public JobObjectBasicLimitInformation BasicLimitInformation;
		public IOCounters IoInfo;
		public UIntPtr ProcessMemoryLimit;
		public UIntPtr JobMemoryLimit;
		public UIntPtr PeakProcessMemoryUsed;
		public UIntPtr PeakJobMemoryUsed;
	}

	private const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 0x2000;

	/// <summary>
	/// Создает Job Object, который автоматически завершит все дочерние процессы при закрытии родительского процесса
	/// </summary>
	private static IntPtr CreateJobObjectForAutoKill()
	{
		// Создаем Job Object
		IntPtr jobHandle = CreateJobObject(IntPtr.Zero, string.Empty);
		if (jobHandle == IntPtr.Zero)
		{
			return IntPtr.Zero;
		}

		// Настраиваем Job Object для автоматического завершения процессов
		var extendedInfo = new JobObjectExtendedLimitInformation();
		extendedInfo.BasicLimitInformation.LimitFlags = JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE;

		int length = Marshal.SizeOf(typeof(JobObjectExtendedLimitInformation));
		IntPtr extendedInfoPtr = Marshal.AllocHGlobal(length);
		
		try
		{
			Marshal.StructureToPtr(extendedInfo, extendedInfoPtr, false);
			
			if (SetInformationJobObject(jobHandle, JobObjectInfoType.ExtendedLimitInformation,
				    extendedInfoPtr, (uint)length))
			{
				return jobHandle;
			}
		}
		finally
		{
			Marshal.FreeHGlobal(extendedInfoPtr);
		}

		// Если настройка не удалась, закрываем handle
		CloseHandle(jobHandle);
		return IntPtr.Zero;
	}
	
	public ServerLauncherHostedService(ILogger<ServerLauncherHostedService> logger)
	{
		_logger = logger;
		_logger.LogInformation("ServerLauncherHostedService constructor called");
		
		// Базовая директория Desktop-приложения
		var currentDir = AppDomain.CurrentDomain.BaseDirectory;

		// 1) Попытка: локальная копия рядом с Desktop.exe (если есть полный комплект файлов)
		string localExe = Path.Combine(currentDir, "Firework.Server.exe");
		string localDir = currentDir;
		bool localOk = File.Exists(localExe) && File.Exists(Path.Combine(localDir, "Firework.Server.runtimeconfig.json"));

		// 2) Попытка: оригинальная папка сборки Firework.Server Debug
		string debugDir = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "..", "Firework.Server", "bin", "Debug", "net8.0"));
		string debugExe = Path.Combine(debugDir, "Firework.Server.exe");
		bool debugOk = File.Exists(debugExe) && File.Exists(Path.Combine(debugDir, "Firework.Server.runtimeconfig.json"));

		// 3) Попытка: оригинальная папка сборки Firework.Server Release
		string releaseDir = Path.GetFullPath(Path.Combine(currentDir, "..", "..", "..", "..", "Firework.Server", "bin", "Release", "net8.0"));
		string releaseExe = Path.Combine(releaseDir, "Firework.Server.exe");
		bool releaseOk = File.Exists(releaseExe) && File.Exists(Path.Combine(releaseDir, "Firework.Server.runtimeconfig.json"));

		if (localOk)
		{
			_workingDirectory = localDir;
			_serverPath = localExe;
		}
		else if (debugOk)
		{
			_workingDirectory = debugDir;
			_serverPath = debugExe;
		}
		else if (releaseOk)
		{
			_workingDirectory = releaseDir;
			_serverPath = releaseExe;
		}
		else
		{
			// В качестве последней попытки — локальная копия exe (может быть self-contained publish)
			_workingDirectory = currentDir;
			_serverPath = localExe;
		}
		
		_logger.LogInformation("Current directory: {CurrentDir}", currentDir);
		_logger.LogInformation("Server working dir: {WorkingDir}", _workingDirectory);
		_logger.LogInformation("Server path: {ServerPath}", _serverPath);
	}
	
	public Task StartAsync(CancellationToken cancellationToken)
	{
		_logger.LogInformation("ServerLauncherHostedService.StartAsync called");
		
		try
		{
			if (!File.Exists(_serverPath))
			{
				_logger.LogError("Server executable not found at: {ServerPath}", _serverPath);
				return Task.CompletedTask;
			}

			// Доп. проверка наличия runtimeconfig для framework-dependent запуска
			var runtimeConfig = Path.Combine(_workingDirectory, "Firework.Server.runtimeconfig.json");
			if (!File.Exists(runtimeConfig))
			{
				_logger.LogWarning("RuntimeConfig not found: {RuntimeConfig}. The app may fail to start.", runtimeConfig);
			}

			_logger.LogInformation("Starting server process from: {ServerPath}", _serverPath);

			ProcessStartInfo processStartInfo = new ProcessStartInfo
			{
				FileName = _serverPath,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				CreateNoWindow = true,
				WorkingDirectory = _workingDirectory
			};
			
			_logger.LogInformation("Creating Job Object for auto-kill functionality...");
			
			// Создаем Job Object для автоматического завершения процесса
			_jobHandle = CreateJobObjectForAutoKill();
			if (_jobHandle == IntPtr.Zero)
			{
				_logger.LogWarning("Failed to create Job Object. Server process may not auto-terminate.");
			}
			else
			{
				_logger.LogInformation("Job Object created successfully");
			}

			_logger.LogInformation("ProcessStartInfo created, starting process...");
			
			_serverProcess = Process.Start(processStartInfo);
			
			if (_serverProcess == null)
			{
				_logger.LogError("Failed to start server process");
				return Task.CompletedTask;
			}
			
			_serverProcess.EnableRaisingEvents = true;
			_logger.LogInformation("Server process started with ID: {ProcessId}", _serverProcess.Id);

			// Назначаем процесс в Job Object для автоматического завершения
			if (_jobHandle != IntPtr.Zero)
			{
				bool assigned = AssignProcessToJobObject(_jobHandle, _serverProcess.Handle);
				if (assigned)
				{
					_logger.LogInformation("Server process successfully assigned to Job Object for auto-termination");
				}
				else
				{
					_logger.LogWarning("Failed to assign server process to Job Object. Process may not auto-terminate.");
				}
			}

			_serverProcess.OutputDataReceived += (sender, args) =>
			{
				if (!string.IsNullOrEmpty(args.Data))
				{
					_logger.LogInformation("Server Output: {Output}", args.Data);
				}
			};
			
			_serverProcess.ErrorDataReceived += (sender, args) =>
			{
				if (!string.IsNullOrEmpty(args.Data))
				{
					_logger.LogError("Server Error: {Error}", args.Data);
				}
			};

			_serverProcess.Exited += (sender, args) =>
			{
				_logger.LogWarning("Server process exited with code: {ExitCode}", _serverProcess?.ExitCode);
			};

			_serverProcess.BeginOutputReadLine();
			_serverProcess.BeginErrorReadLine();
			
			_logger.LogInformation("ServerLauncherHostedService.StartAsync completed successfully");
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error starting server process");
		}
		
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		KillServer();
		return Task.CompletedTask;
	}
	
	private void KillServer()
	{
		if (_serverProcess is { HasExited: false })
		{
			_logger.LogInformation("Killing server process with ID: {ProcessId}", _serverProcess.Id);
			
			try
			{
				_serverProcess.Kill();
				_serverProcess.WaitForExit(5000); // Ждем максимум 5 секунд
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error killing server process");
			}
			finally
			{
				_serverProcess?.Dispose();
				_serverProcess = null;
			}
		}

		// Закрываем Job Object handle
		if (_jobHandle != IntPtr.Zero)
		{
			_logger.LogInformation("Closing Job Object handle");
			CloseHandle(_jobHandle);
			_jobHandle = IntPtr.Zero;
		}
	}

	private Dictionary<string, string> GetConfig()
	{
		var dictionary = new Dictionary<string, string>
		{ };
		
		return dictionary;
	}
	
	
	private bool IsServerRunning()
	{
		return _serverProcess is { HasExited: false };
	}
}