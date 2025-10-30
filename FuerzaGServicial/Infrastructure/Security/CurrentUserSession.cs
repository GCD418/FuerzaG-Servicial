using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;

namespace FuerzaGServicial.Infrastructure.Security;

public class CurrentUserSession : ISessionManager
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserSession(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public async Task Login(UserAccount userAccount)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userAccount.Id.ToString()),
            new Claim(ClaimTypes.Role, userAccount.Role),
            new Claim(ClaimTypes.Name, userAccount.Name),
            new Claim("UserName", userAccount.UserName),
            new Claim("FirstLastName", userAccount.FirstLastName),
            new Claim("SecondLastName", userAccount.SecondLastName ?? string.Empty),
            new Claim(ClaimTypes.Email, userAccount.Email ?? string.Empty) 
        };

        var identity = new ClaimsIdentity(claims, "GForceAuth", ClaimTypes.Name, ClaimTypes.Role);
        var principal = new  ClaimsPrincipal(identity);

        await _httpContextAccessor.HttpContext.SignInAsync(
            "GForceAuth",
            principal,
            new AuthenticationProperties { IsPersistent = false }
        );

    }

    public async Task Logout()
    {
        await _httpContextAccessor.HttpContext.SignOutAsync("GForceAuth");
    }

    public int? UserId
    {
        get
        {
            var idClaim =
                _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                _httpContextAccessor.HttpContext?.User?.FindFirst("sub")?.Value;

            if (int.TryParse(idClaim, out var userId))
                return userId;

            return null;
        }
    }

    public string FullName
    {
        get
        {
            var name = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            var firstLastName = _httpContextAccessor.HttpContext.User.FindFirst("FirstLastName")?.Value;
            var secondLastName = _httpContextAccessor.HttpContext.User.FindFirst("SecondLastName")?.Value;
            return $"{name} {firstLastName} {secondLastName}";
        }
    }
}