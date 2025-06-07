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

#region Services

builder.Services.AddScoped<IInstructionService, InstructionService>();
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.AddSingleton<INetEventService, NetEventService>();

builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();
builder.Services.AddScoped<IMacroLauncher, MacroLauncher>();
builder.Services.AddScoped<IServiceManager, ServiceManager>();

#endregion

#region DataBase

builder.Services.AddScoped<IDataRepository<SettingsItem>, SettingsRepository>();
builder.Services.AddScoped<IDataRepository<Metadata>, MetadataRepository>();
builder.Services.AddScoped<DbRepository>();

#endregion

ServiceManager serviceManager = new();

#region InstructionServices

builder.Services.AddService<AppService>(serviceManager);
builder.Services.AddService<OsService>(serviceManager);
builder.Services.AddService<KeyboardService>(serviceManager);
builder.Services.AddService<MouseService>(serviceManager);
builder.Services.AddService<TaskService>(serviceManager);

#endregion

#region NetworkConfiguration

builder.Services.AddRouting();
builder.Services.AddSignalR();

#endregion


var app = builder.Build();

serviceManager.ServiceProvider = app.Services;

app.MapHub<SignalHub>("/signal");
app.MapGet("/health", () => "ok");

app.Run();


namespace Firework.Server
{
    static class ServiceCollectionExtend
    {
        public static IServiceCollection AddService<T>(this IServiceCollection services, IServiceManager serviceManager) where T : class, IServiceBase
        {
            serviceManager.AddService<T>();

            services.AddSingleton<T>();

            return services;
        }
    }
}