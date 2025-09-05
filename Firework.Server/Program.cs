using Firework.Abstraction.Connection;
using Firework.Abstraction.Data;
using Firework.Abstraction.Instruction;
using Firework.Abstraction.MacroLauncher;
using Firework.Abstraction.Services;
using Firework.Abstraction.Services.FileService;
using Firework.Abstraction.Services.NetEventService;
using Firework.Core;
using Firework.Core.Data;
using Firework.Core.Instruction;
using Firework.Core.Macro;
using Firework.Core.MacroServices;
using Firework.Core.Services;
using Firework.Core.Services.PipeBroker;
using Firework.Models.Data;
using Firework.Models.Metadata;
using Firework.Server;
using Firework.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Добавляем логирование
builder.Logging.AddConsole();
builder.Logging.AddDebug();

#region Services

builder.Services.AddScoped<IInstructionService, InstructionService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddSingleton<INetEventService, NetEventService>();

builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();
builder.Services.AddSingleton<IDeviceConnectionService, DeviceConnectionService>();
builder.Services.AddSingleton<IDeviceAuthorizationService, DeviceAuthorizationService>();
builder.Services.AddScoped<IMacroLauncher, MacroLauncher>();
builder.Services.AddSingleton<IServiceManager, ServiceManager>();
builder.Services.AddSingleton<ServiceManager>();

#endregion

builder.Configuration
    .AddEnvironmentVariables()
    .AddCommandLine(args);

#region DataBase

builder.Services.AddScoped<IDataRepository<SettingsItem>, SettingsRepository>();
builder.Services.AddScoped<IDataRepository<Metadata>, MetadataRepository>();
builder.Services.AddScoped<DbRepository>();

#endregion

#region InstructionServices

// Инициализация ServiceManager после построения приложения
var serviceManager = new ServiceManager();

builder.Services.AddService<AppService>(serviceManager);
builder.Services.AddService<OsService>(serviceManager);
builder.Services.AddService<KeyboardService>(serviceManager);
builder.Services.AddService<MouseService>(serviceManager);
builder.Services.AddService<TaskService>(serviceManager);

#endregion

#region NetworkConfiguration

builder.Services.AddRouting();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

#endregion

var app = builder.Build();

serviceManager.ServiceProvider = app.Services;

// Добавляем middleware для обработки ошибок
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();

app.MapControllers();
app.MapHub<SignalHub>("/signal");
app.MapGet("/health", () => "ok");

app.Logger.LogInformation("Firework.Server запущен на {Time}", DateTime.UtcNow);

app.Run();


namespace Firework.Server
{
    static class ServiceCollectionExtend
    {
        public static IServiceCollection AddService<T>(this IServiceCollection services, ServiceManager serviceManager) where T : class, IServiceBase
        {
            services.AddScoped<T>();
            serviceManager.AddService<T>();
            
            return services;
        }
    }
}