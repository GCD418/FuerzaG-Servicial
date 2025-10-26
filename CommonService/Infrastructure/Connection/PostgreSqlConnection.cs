using System.Data.Common;
using Npgsql;

namespace CommonService.Infrastructure.Connection;

public class PostgreSqlConnection : IDbConnectionFactory
{
    private readonly DatabaseConnectionManager _connectionManager;

    public PostgreSqlConnection(DatabaseConnectionManager connectionManager)
    {
        _connectionManager = connectionManager;
    }

    public DbConnection CreateConnection()
    {
        return new NpgsqlConnection(_connectionManager.ConnectionString);
    }

    public string GetProviderName()
    {
        return "PostgreSql";
    }}