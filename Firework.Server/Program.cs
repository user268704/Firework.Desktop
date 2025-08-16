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

builder.Services.AddService<AppService>();
builder.Services.AddService<OsService>();
builder.Services.AddService<KeyboardService>();
builder.Services.AddService<MouseService>();
builder.Services.AddService<TaskService>();

#endregion

#region NetworkConfiguration

builder.Services.AddRouting();
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
    options.EnableDetailedErrors = builder.Environment.IsDevelopment();
    options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
    options.KeepAliveInterval = TimeSpan.FromSeconds(15);
});

#endregion

var app = builder.Build();

// Инициализация ServiceManager после построения приложения
var serviceManager = app.Services.GetRequiredService<ServiceManager>();
serviceManager.ServiceProvider = app.Services;

// Добавляем middleware для обработки ошибок
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseCors();

app.MapHub<SignalHub>("/signal");
app.MapGet("/health", () => "ok");

app.Logger.LogInformation("Firework.Server запущен на {Time}", DateTime.UtcNow);

app.Run();


namespace Firework.Server
{
    static class ServiceCollectionExtend
    {
        public static IServiceCollection AddService<T>(this IServiceCollection services) where T : class, IServiceBase
        {
            services.AddSingleton<T>();
            return services;
        }
    }
}