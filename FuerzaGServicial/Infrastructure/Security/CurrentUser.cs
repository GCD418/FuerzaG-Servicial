using System.Security.Claims;
using CommonService.Domain.Ports;
using Microsoft.AspNetCore.Authentication;

namespace FuerzaGServicial.Infrastructure.Security;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public bool IsAuthenticated =>
        _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated == true;

    public async Task SetUpUserSession(IUserAccountSession session)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, session.Id.ToString()),
            new Claim(ClaimTypes.Name, session.UserName), 
            new Claim(ClaimTypes.Role, session.Role),
            new Claim("FirstLastName", session.FirstLastName),
            new Claim("SecondLastName", session.SecondLastName ?? string.Empty),
            new Claim(ClaimTypes.Email, session.Email ?? string.Empty) 
        };

        var identity = new ClaimsIdentity(claims, "GForceAuth", ClaimTypes.Name, ClaimTypes.Role);
        var principal = new  ClaimsPrincipal(identity);

        await _httpContextAccessor.HttpContext.SignInAsync(
            "GForceAuth",
            principal,
            new AuthenticationProperties { IsPersistent = false }
        );

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
}