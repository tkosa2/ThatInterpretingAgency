using Microsoft.AspNetCore.Mvc;

namespace Bff.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProxyTestController : ControllerBase
{
    private readonly ILogger<ProxyTestController> _logger;

    public ProxyTestController(ILogger<ProxyTestController> logger)
    {
        _logger = logger;
    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        _logger.LogInformation("BFF Proxy Test endpoint called");
        return Ok(new
        {
            Message = "BFF Reverse Proxy is working!",
            Timestamp = DateTime.UtcNow,
            BffPort = "7002",
            ApiPort = "7058",
            Status = "Active"
        });
    }

    [HttpGet("proxy-info")]
    public IActionResult GetProxyInfo()
    {
        return Ok(new
        {
            ProxyType = "Reverse Proxy",
            FrontendPorts = new[] { "7263", "5096", "7001", "5001" },
            BffPort = "7002",
            ApiDestination = "https://localhost:7058",
            Routes = new[]
            {
                "/api/agencies/**",
                "/api/interpreters/**",
                "/api/clients/**",
                "/api/appointments/**",
                "/api/billing/**",
                "/api/admin/**",
                "/api/notifications/**",
                "/api/staff/**",
                "/api/interpreter-requests/**",
                "/swagger/**"
            },
            Features = new[]
            {
                "Request Proxying",
                "Health Checks",
                "Custom Headers",
                "Request Logging",
                "CORS Support"
            }
        });
    }
}
