namespace CommonService.Infrastructure.Connection;

public class PostgreSqlConnection
{
    private readonly DatabaseConnectionManager _connectionManager;

    public PostgreSqlConnection(DatabaseConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionManager.ConnectionString);
    }

    public string GetProviderName()
    {
        return "PostgreSql";
    }
    
}