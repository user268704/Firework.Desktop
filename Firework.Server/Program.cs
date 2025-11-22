using Firework.Server.Abstraction;
using Firework.Server.Configuration;
using Firework.Server.Endpoints;
using Firework.Server.Filters;
using Firework.Server.Hubs;
using Firework.Server.Logging;
using Firework.Server.Modules;
using Firework.Server.Services;
using Firework.Server.Validators;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("serverconfig.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args);

using var bootstrapLoggerFactory = LoggerFactory.Create(logging => logging.AddSimpleConsole());
var bootstrapLogger = bootstrapLoggerFactory.CreateLogger<ServerConfigurationProvider>();
var serverConfigurationProvider = new ServerConfigurationProvider(builder.Environment, bootstrapLogger);

ConfigureLogging(builder.Logging, serverConfigurationProvider.Current.Logging);

builder.Services.AddSingleton<IServerConfigurationProvider>(serverConfigurationProvider);
builder.Services.AddSingleton<IAccessCodeService, AccessCodeService>();
builder.Services.AddSingleton<IClientRegistry, ClientRegistry>();
builder.Services.AddSingleton<IMessagePackService, MessagePackService>();
builder.Services.AddSingleton<ClientAuthorizationFilter>();

builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<ICommandDispatcher, CommandDispatcher>();
builder.Services.AddScoped<ISystemStateService, SystemStateService>();

builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();

var moduleTypes = new[] {
    typeof(SystemModule), 
    typeof(DiagnosticsModule) 
};

foreach (var moduleType in moduleTypes)
{
    builder.Services.AddScoped(moduleType);
}

builder.Services.AddSingleton<ICommandModuleRegistry>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<CommandModuleRegistry>>();
    return new CommandModuleRegistry(moduleTypes, logger);
});

/*builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});*/

builder.Services.AddSignalR(options =>
    {
        options.EnableDetailedErrors = true;
        options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
        options.KeepAliveInterval = TimeSpan.FromSeconds(15);
    })
    .AddMessagePackProtocol(options =>
    {
        options.SerializerOptions = MessagePackService.SharedOptions;
    });

var app = builder.Build();

app.UseCors();

app.MapHub<SignalHub>("/signal");

app.MapGroup("api")
    .MapApiEndpoints();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Firework.Server started at {Time}. Current access code: {AccessCode}",
    DateTime.UtcNow,
    serverConfigurationProvider.Current.Security.AccessCode);

app.Run();

var welcomeText = new WelcomeService().GetWelcomeText();

logger.LogInformation(welcomeText);


static void ConfigureLogging(ILoggingBuilder loggingBuilder, LoggingOptions loggingOptions)
{
    loggingBuilder.ClearProviders();

    if (!loggingOptions.Enabled)
    {
        return;
    }

    if (loggingOptions.LogToConsole)
    {
        loggingBuilder.AddSimpleConsole();
    }

    if (loggingOptions.LogToFile && !string.IsNullOrWhiteSpace(loggingOptions.FilePath))
    {
        loggingBuilder.AddProvider(new FileLoggerProvider(loggingOptions.FilePath));
    }
}