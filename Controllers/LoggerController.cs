using Microsoft.AspNetCore.Mvc;
using ILogger = GerberBackend.Core.Contracts.ILogger;

namespace GerberBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoggerController : ControllerBase
{
    private readonly ILogger _logger;

    public LoggerController(ILogger logger)
    {
        _logger = logger;
    }

    [HttpPost("set-log/{userId}")]
    public async Task<IActionResult> GetFiles([FromBody] string errorMessage, string userId)
    {
        await _logger.LogErrorClientAsync(errorMessage, userId);

        return Ok();
    }
}
