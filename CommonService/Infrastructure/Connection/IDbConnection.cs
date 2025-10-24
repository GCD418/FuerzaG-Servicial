namespace CommonService.Infrastructure.Connection;

public interface IDbConnection
{
    IDbConnection CreateConnection();
    
    string GetProviderName();
}
    