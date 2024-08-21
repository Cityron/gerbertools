using GerberBackend.Contracts;
using GerberBackend.Core.Contracts;
using GerberBackend.Utils;
using Microsoft.AspNetCore.Mvc;

namespace GerberBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SessionFilesController : ControllerBase
{
    private IHttpContextAccessor _httpContextAccessor;
    private readonly ISessionStore _sessionStore;

    private readonly SessionToken _token = new();
    public SessionFilesController(IHttpContextAccessor httpContextAccessor, ISessionStore store)
    {
        _httpContextAccessor = httpContextAccessor;
        _sessionStore = store;
    }

    [HttpGet("get-session-file")]
    public IActionResult GetFiles()
    {
        var authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

        string sessionId = _token.GetSessionIdFromToken(authHeader);

       var files = _sessionStore.GetFiles(Guid.Parse(sessionId));

        return Ok(files);
    }

    [HttpDelete("delete")]
    public IActionResult DeleteFiles() {

        var authHeader = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].FirstOrDefault();

        string sessionId = _token.GetSessionIdFromToken(authHeader);

        _sessionStore.RemoveFile(Guid.Parse(sessionId));

        return Ok();
    }
}
