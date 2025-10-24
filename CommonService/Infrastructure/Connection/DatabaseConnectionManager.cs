namespace CommonService.Infrastructure.Connection;

public class DatabaseConnectionManager
{
    private static DatabaseConnectionManager _instance;
    private static readonly object Locker = new object();
    public string ConnectionString { get; private set; }

    private DatabaseConnectionManager(string  connectionString)
    {
        ConnectionString = connectionString;
    }

    public static DatabaseConnectionManager GetInstance(string connectionString)
    {
        if (_instance == null)
        {
            lock (Locker)
            {
                _instance ??= new DatabaseConnectionManager(connectionString);
            }
        }
        return _instance;
    }

    public static DatabaseConnectionManager GetInstance()
    {
        return _instance ?? throw new Exception("Database connection manager not initialized");
    }

    public void UpdateConnectionString(string connectionString)
    {
        lock (Locker)
        {
            ConnectionString = connectionString;
        }
    }
    
}