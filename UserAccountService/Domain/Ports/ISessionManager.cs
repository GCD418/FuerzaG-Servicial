using UserAccountService.Domain.Entities;

namespace UserAccountService.Domain.Ports;

public interface ISessionManager
{
    Task Login(UserAccount userAccount);
    Task Logout();
    public bool IsAuthenticated { get; }
    public int? UserId { get; }
    public string FullName { get; }
}