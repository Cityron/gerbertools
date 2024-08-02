using GerberBackend.Core.Contracts;
using GerberBackend.Core.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GerberBackend.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("seed-roles")]
    public async Task<IActionResult> SeedRoles()
    {
        var seedRoles = await _authService.SeedRolesAsync();
        return StatusCode(seedRoles.StatusCode, seedRoles.Message);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Registr([FromBody] RegisterDto dto)
    {
        var registerResult = await _authService.RegisterAsync(dto);
        return StatusCode(registerResult.StatusCode, registerResult.Message);
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginServiceResponseDto>> Login([FromBody] LoginDto dto)
    {
        var loginResult = await _authService.LoginAsync(dto);
        if (loginResult is null)
            return Unauthorized("Неверный логин или пароль. Пожалуйста, обратитесь к администратору");

        return Ok(loginResult);
    }

    [HttpPost("update-role")]
    [Authorize(Roles = StaticUserRoles.OwnerAdmin)]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto dto)
    {
        var updateRolesResult = await _authService.UpdateRoleAsync(User, dto);

        if (updateRolesResult.IsSucced)
            return Ok(updateRolesResult.Message);
        else
            return StatusCode(updateRolesResult.StatusCode, updateRolesResult.Message);
    }

    [HttpPost("me")]
    public async Task<ActionResult<LoginServiceResponseDto>> Me([FromBody] MeDto token)
    {
        var me = await _authService.MeAsync(token);
        if (me is not null)
            return Ok(me);
        else
            return Unauthorized("Неверный токен");
    }

    [HttpGet("users")]
    [Authorize(Roles = StaticUserRoles.OwnerAdmin)]
    public async Task<ActionResult<IEnumerable<UserInfoResult>>> GetUsersList()
    {
        var userList = await _authService.GetUsersListAsync();

        return Ok(userList);
    }

    [HttpGet("user")]
    public async Task<ActionResult<UserInfoResult>> GetUserDetailsByUserName([FromBody] string userName)
    {
        var user = await _authService.GetUserDetailsByUserNameAsync(userName);
        if (user is not null)
            return Ok(user);
        else
            return NotFound("Пользователь не найден");
    }

    [HttpGet("usernames")]
    public async Task<ActionResult<IEnumerable<string>>> GetUserNamesList()
    {
        var usernames = await _authService.GetUsernamesListAsync();

        return Ok(usernames);
    }
}
