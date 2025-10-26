using System.Data.Common;

namespace CommonService.Infrastructure.Connection;

public interface IDbConnectionFactory
{
    DbConnection  CreateConnection();

    string GetProviderName();
    
}