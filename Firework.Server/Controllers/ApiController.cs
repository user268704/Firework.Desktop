using Firework.Dto.Devices;
using Firework.Server.Abstraction;
using Firework.Server.Services;
using Microsoft.AspNetCore.Mvc;

namespace Firework.Server.Controllers;

[ApiController]
[Route("api")]
public class ApiController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ICommandExecutor _commandExecutor;
    private readonly ICommandsService _commandsService;
    private readonly RequestContextService _requestContextService;

    public ApiController(IAuthenticationService authenticationService,
        ICommandExecutor commandExecutor,
        ICommandsService commandsService,
        RequestContextService requestContextService)
    {
        _authenticationService = authenticationService;
        _commandExecutor = commandExecutor;
        _commandsService = commandsService;
        _requestContextService = requestContextService;
    }
    
    [HttpPost("register")]
    public IActionResult Register(DeviceRegisterDto device)
    {
        var requestContext = _requestContextService.CreateContextFromHttpContext(HttpContext);
        var result = _authenticationService.Authenticate(device, requestContext);
        
        return Ok(new
        {
            Status = "ok",
            Token = result
        });
    }

    [HttpPost("login")]
    public IActionResult Logout(string deviceHash)
    {
        throw new NotImplementedException();
        
    }

    [HttpPost("command")]
    public IActionResult ExecuteCommand(string command)
    {
        var isUserAuth = HttpContext.Request.Headers.TryGetValue("Authorization", out var value);

        if (isUserAuth == false)
        {
            return Unauthorized("Invalid or missing authentication token.");
        }
        
        var device = _authenticationService.GetDevice(value);

        if (device == null)
        {
            return Unauthorized("You must register");
        }
        
        var commandResult = _commandExecutor.ExecuteCommand(command, device);

        return Ok(commandResult);
    }

    [HttpGet("health")]
    public IActionResult HealthStatus()
    {
        throw new NotImplementedException();
        
    }

    [HttpGet("status")]
    public IActionResult StatusConnection()
    {
        throw new NotImplementedException();
        
    }

    [HttpGet("services")]
    public IActionResult GetServices()
    {
        var allServices = _commandsService.GetAllServices();
        
        

        return Ok();
    }
}