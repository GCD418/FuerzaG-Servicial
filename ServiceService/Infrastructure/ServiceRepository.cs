using System.Data;
using FuerzaG.Domain.Ports;
using FuerzaG.Domain.Entities;
using FuerzaG.Infrastructure.Connection;
using FuerzaG.Infrastructure.Security; 

namespace FuerzaG.Infrastructure.Persistence;

public class ServiceRepository : IRepository<Service>
{
    private readonly IDbConnectionFactory _dbConnectionFactory;
    private readonly ICurrentUser _currentUser; 

    public ServiceRepository(IDbConnectionFactory dbConnectionFactory, ICurrentUser currentUser)
    {
        _dbConnectionFactory = dbConnectionFactory;
        _currentUser = currentUser;
    }

    public List<Service> GetAll()
    {
        var services = new List<Service>();
        using var connection = _dbConnectionFactory.CreateConnection();

        string query = "SELECT * FROM fn_get_active_services()";

        using var command = connection.CreateCommand();
        command.CommandText = query;
        connection.Open();

        using var reader = command.ExecuteReader();
        while (reader.Read())
            services.Add(MapReaderToModel(reader));

        return services;
    }

    public Service? GetById(int id)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT * FROM fn_get_service_by_id(@id)";

        using var command = connection.CreateCommand();
        command.CommandText = query;
        AddParameter(command, "@id", id);

        connection.Open();
        using var reader = command.ExecuteReader();
        return reader.Read() ? MapReaderToModel(reader) : null;
    }

    public int Create(Service service)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_insert_service(@name, @type, @price, @description, @created_by_user_id)";

        using var command = connection.CreateCommand();
        command.CommandText = query;

        AddParameter(command, "@name", service.Name);
        AddParameter(command, "@type", service.Type);
        AddParameter(command, "@price", service.Price);
        AddParameter(command, "@description", service.Description);
        AddParameter(command, "@created_by_user_id", _currentUser.UserId ?? -1);

        connection.Open();
        var idObj = command.ExecuteScalar();
        return Convert.ToInt32(idObj);
    }

    public bool Update(Service service)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_update_service(@id, @name, @type, @price, @description, @modified_by_user_id)";

        using var command = connection.CreateCommand();
        command.CommandText = query;

        AddParameter(command, "@id", service.Id);
        AddParameter(command, "@name", service.Name);
        AddParameter(command, "@type", service.Type);
        AddParameter(command, "@price", service.Price);
        AddParameter(command, "@description", service.Description);
        AddParameter(command, "@modified_by_user_id", _currentUser.UserId ?? -1);

        connection.Open();
        return Convert.ToBoolean(command.ExecuteScalar());
    }

    public bool DeleteById(int id)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_soft_delete_service(@id, @modified_by_user_id)";
        using var command = connection.CreateCommand();
        command.CommandText = query;
        AddParameter(command, "@id", id);
        AddParameter(command, "@modified_by_user_id", _currentUser.UserId ?? -1); 
        connection.Open();
        return Convert.ToBoolean(command.ExecuteScalar());
    }

    public Service MapReaderToModel(IDataReader reader)
    {
        return new Service
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Type = reader.GetString(2),
            Price = reader.GetDecimal(3),
            Description = reader.GetString(4),
            CreatedAt = reader.GetDateTime(5),
            UpdatedAt = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
            IsActive = reader.GetBoolean(7),
            ModifiedByUserId = reader.IsDBNull(8) ? null : reader.GetInt32(8)
        };
    }

    private void AddParameter(IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}