using System.Data;

namespace CommonService.Infrastructure.Connection;

public interface IDbConnectionFactory
{
    IDbConnection  CreateConnection();

    string GetProviderName();
    
}