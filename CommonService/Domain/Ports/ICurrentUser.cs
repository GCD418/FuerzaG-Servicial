namespace CommonService.Domain.Ports;

public interface ICurrentUser
{
    int? UserId { get; }
    
    bool IsAuthenticated { get; }

    Task SetUpUserSession(IUserAccountSession session);
}