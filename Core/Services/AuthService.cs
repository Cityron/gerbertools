﻿using GerberBackend.Config;
using GerberBackend.Core.Contracts;
using GerberBackend.Core.Dtos.Auth;
using GerberBackend.Core.Dtos.General;
using GerberBackend.Core.Entities.Auth;
using GerberBackend.Core.Entities.UserStoreCustom;
using GerberBackend.Core.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GerberBackend.Core.Services;

public class AuthService : IAuthService
{

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly UserStoreCustom _userStoreCustom;
    private readonly ISessionStore _sessionStore;
    private readonly Contracts.ILogger _logger;

    public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, 
        IConfiguration configuration, UserStoreCustom userStoreCustom, ISessionStore store, Contracts.ILogger logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _userStoreCustom = userStoreCustom;
        _sessionStore = store;
        _logger = logger;
    }

    public async Task<GeneralServiceResponseDto> SeedRolesAsync()
    {
        bool isOwnerRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.Owner);
        bool isAdminRoleExists = await _roleManager.RoleExistsAsync(StaticUserRoles.Admin);
        bool isManager = await _roleManager.RoleExistsAsync(StaticUserRoles.Manager);
        bool isUser = await _roleManager.RoleExistsAsync(StaticUserRoles.User);

        if (isOwnerRoleExists && isAdminRoleExists && isManager && isUser)
            return new GeneralServiceResponseDto()
            {
                IsSucced = true,
                StatusCode = 200,
                Message = "Все роли уже существуют"
            };

        await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.Owner));
        await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.Admin));
        await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.Manager));
        await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.User));

        return new GeneralServiceResponseDto()
        {
            IsSucced = true,
            StatusCode = 201,
            Message = "Все роли добавлены"
        };
    }

    public async Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        var isExistsUser = await _userManager.FindByNameAsync(registerDto.UserName);
        var isExistsUserPhone = await _userStoreCustom.FindByPhoneNumberAsync(registerDto.PhoneNumber);

        if (isExistsUser is not null)
            return new GeneralServiceResponseDto()
            {
                IsSucced = false,
                StatusCode = 409,
                Message = "Пользователь с таким именем уже существует"
            };

        if (isExistsUserPhone is not null)
            return new GeneralServiceResponseDto()
            {
                IsSucced = false,
                StatusCode = 409,
                Message = "Пользователь с таким номером телефона уже существует"
            };


        ApplicationUser newUser = new ApplicationUser()
        {
            FirstName = registerDto.FirstName,
            SecondName = registerDto.SecondName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            UserName = registerDto.UserName,
            City = registerDto.City,
            PhoneNumber = registerDto.PhoneNumber,
            SecurityStamp = Guid.NewGuid().ToString(),
        };


        var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);

        if (!createUserResult.Succeeded)
        {
            var errorString = "Ошибка создания пользователя";
            foreach (var error in createUserResult.Errors)
            {
                errorString += " #" + error.Description;
            }

            await _logger.LogErrorServerAsync(errorString);

            return new GeneralServiceResponseDto()
            {
                IsSucced = false,
                StatusCode = 400,
                Message = errorString
            };
        }



        await _userManager.AddToRoleAsync(newUser, StaticUserRoles.Owner);

        await _logger.LogUserActionAsync(newUser.Id, $"{newUser.UserName} успешно зарегистрирован");

        return new GeneralServiceResponseDto()
        {
            IsSucced = true,
            StatusCode = 201,
            Message = "Пользователь успешно зарегистрирован!"
        };

    }


    public async Task<LoginServiceResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userManager.FindByNameAsync(loginDto.UserName);
        if (user is null)
            return null;

        var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
        if (!isPasswordCorrect)
            return null;

        var newToken = await GenerateJWTTokenAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var userInfo = GenerateUserInfoObject(user, roles);

        await _logger.LogUserActionAsync(user.UserName, "Выполнил вход");

        return new LoginServiceResponseDto()
        {
            NewToken = newToken,
            UserInfo = userInfo
        };

    }

    public async Task<GeneralServiceResponseDto> UpdateRoleAsync(ClaimsPrincipal User, UpdateRoleDto updateRoleDto)
    {
        var user = await _userManager.FindByNameAsync(updateRoleDto.UserName);
        if (user is null)
            return new GeneralServiceResponseDto()
            {
                IsSucced = false,
                StatusCode = 404,
                Message = "Неверное имя пользователя"
            };

        var userRoles = await _userManager.GetRolesAsync(user);

        if (User.IsInRole(StaticUserRoles.Admin))
        {
            if (updateRoleDto.NewRole.Equals(RoleType.User) || updateRoleDto.NewRole.Equals(RoleType.Manager))
            {
                if (userRoles.Any(q => q.Equals(StaticUserRoles.Owner) || q.Equals(StaticUserRoles.Admin)))
                {
                    return new GeneralServiceResponseDto()
                    {
                        IsSucced = false,
                        StatusCode = 403,
                        Message = "Нет доступа к изменению роли пользователя",
                    };
                }
                else
                {
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                    await _userManager.AddToRoleAsync(user, updateRoleDto.NewRole.ToString());
                    await _logger.LogUserActionAsync(null, "Роли успешно обновлены");
                    return new GeneralServiceResponseDto()
                    {
                        IsSucced = true,
                        StatusCode = 200,
                        Message = "Роли успешно обновлены",
                    };
                }
            }
            else return new GeneralServiceResponseDto()
            {
                IsSucced = false,
                StatusCode = 403,
                Message = "Нет доступа к изменению роли пользователя"
            };

        }
        else
        {
            if (userRoles.Any(q => q.Equals(StaticUserRoles.Owner)))
            {
                return new GeneralServiceResponseDto()
                {
                    IsSucced = false,
                    StatusCode = 403,
                    Message = "Нет доступа к изменению роли пользователя"
                };
            }

            else
            {
                await _userManager.RemoveFromRolesAsync(user, userRoles);
                await _userManager.AddToRoleAsync(user, updateRoleDto.NewRole.ToString());

                return new GeneralServiceResponseDto()
                {
                    IsSucced = true,
                    StatusCode = 200,
                    Message = "Роли успешно обновлены"
                };
            }

        }
    }
    public async Task<LoginServiceResponseDto> MeAsync(MeDto meDto)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["JWT:ValidIssuer"],
            ValidAudience = _configuration["JWT:ValidAudience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"])),
            ClockSkew = TimeSpan.Zero
        };

        try
        {
            var handler = tokenHandler.ValidateToken(meDto.Token, validationParameters, out SecurityToken securityToken);

            var decodedUserName = handler.Claims.FirstOrDefault(q => q.Type == ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(decodedUserName))
                return null;

            var user = await _userManager.FindByNameAsync(decodedUserName);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfoObject(user, roles);

            var decodedSession = handler.Claims.FirstOrDefault(q => q.Type == CustomClaimTypes.SessionId)?.Value;

            var session = _sessionStore.IsValidSession(Guid.Parse(decodedSession));

            if (session is SessionData)
            {
                return new LoginServiceResponseDto
                {
                    NewToken = meDto.Token,
                    UserInfo = userInfo
                };
            }
            else
            {
                var newToken = await GenerateJWTTokenAsync(user);

                return new LoginServiceResponseDto
                {
                    NewToken = newToken,
                    UserInfo = userInfo
                };
            }
        }
        catch (SecurityTokenExpiredException)
        {
            var handler = new JwtSecurityTokenHandler().ReadJwtToken(meDto.Token);
            var decodedUserName = handler.Claims.FirstOrDefault(q => q.Type == ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(decodedUserName))
                return null;

            var user = await _userManager.FindByNameAsync(decodedUserName);
            if (user == null)
                return null;

            var newToken = await GenerateJWTTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfoObject(user, roles);

            return new LoginServiceResponseDto
            {
                NewToken = newToken,
                UserInfo = userInfo
            };
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            var handler = new JwtSecurityTokenHandler().ReadJwtToken(meDto.Token);
            var decodedUserId = handler.Claims.FirstOrDefault(q => q.Type == CustomClaimTypes.Id)?.Value;
            await _logger.LogErrorServerAsync("Недействительный токен", decodedUserId);
            return null;
        }
        catch (SecurityTokenInvalidAudienceException)
        {
            var handler = new JwtSecurityTokenHandler().ReadJwtToken(meDto.Token);
            var decodedUserId = handler.Claims.FirstOrDefault(q => q.Type == CustomClaimTypes.Id)?.Value;
            await _logger.LogErrorServerAsync("Недействительный токен", decodedUserId);
            return null;
        }
        catch (SecurityTokenInvalidIssuerException)
        {
            var handler = new JwtSecurityTokenHandler().ReadJwtToken(meDto.Token);
            var decodedUserId = handler.Claims.FirstOrDefault(q => q.Type == CustomClaimTypes.Id)?.Value;
            await _logger.LogErrorServerAsync("Недействительный токен", decodedUserId);
            return null;
        }
        catch (SecurityTokenException)
        {
            var handler = new JwtSecurityTokenHandler().ReadJwtToken(meDto.Token);
            var decodedUserId = handler.Claims.FirstOrDefault(q => q.Type == CustomClaimTypes.Id)?.Value;
            await _logger.LogErrorServerAsync("Недействительный токен", decodedUserId);
            return null;
        }
        catch (Exception ex)
        {
            await _logger.LogErrorServerAsync($"An error occurred: {ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<UserInfoResult>> GetUsersListAsync()
    {
        var users = await _userManager.Users.ToListAsync();

        List<UserInfoResult> userInfoResults = new List<UserInfoResult>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfoObject(user, roles);
            userInfoResults.Add(userInfo);
        }

        return userInfoResults;
    }

    public async Task<UserInfoResult> GetUserDetailsByUserNameAsync(string userName)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user is null)
            return null;

        var roles = await _userManager.GetRolesAsync(user);
        var userInfo = GenerateUserInfoObject(user, roles);

        return userInfo;
    }

    public async Task<IEnumerable<string>> GetUsernamesListAsync()
    {
        var userNames = await _userManager.Users.Select(q => q.UserName).ToListAsync();

        return userNames;
    }

    private async Task<string> GenerateJWTTokenAsync(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);

        var sessionStore = _sessionStore.GetSession(Guid.NewGuid());

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(CustomClaimTypes.Id, user.Id),
            new Claim("FirstName", user.FirstName),
            new Claim("LastName", user.LastName),
            new Claim(CustomClaimTypes.SessionId, sessionStore.SessionId.ToString())
        };

        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));
        var signinCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);

        var tokenObject = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            notBefore: DateTime.Now,
            expires: DateTime.Now.AddHours(5),
            claims: authClaims,
            signingCredentials: signinCredentials
            );


        string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);
        return token;
    }

    private UserInfoResult GenerateUserInfoObject(ApplicationUser user, IEnumerable<string> Roles)
    {
        return new UserInfoResult()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            SecondName = user.SecondName,
            LastName = user.LastName,
            UserName = user.UserName,
            City = user.City,
            Email = user.Email,
            Roles = Roles,
            CreatedAt = user.CreatedAt,
            PhoneNumber = user.PhoneNumber,
        };
    }


}
