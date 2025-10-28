using System.Data;
using CommonService.Infrastructure.Connection;
using ServiceService.Domain.Entities;
using ServiceService.Domain.Ports;

namespace ServiceService.Infrastructure.Persistence;

public class ServiceRepository : IServiceRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public ServiceRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<Service>> GetAllAsync()
    {
        var services = new List<Service>();
        await using var connection = _dbConnectionFactory.CreateConnection();

        const string query = "SELECT * FROM fn_get_active_services()";

        await using var command = connection.CreateCommand();
        command.CommandText = query;

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            services.Add(MapReaderToModel(reader));

        return services;
    }

    public async Task<Service?> GetByIdAsync(int id)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT * FROM fn_get_service_by_id(@id)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;
        AddParameter(command, "@id", id);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        return await reader.ReadAsync() ? MapReaderToModel(reader) : null;
    }

    public async Task<bool> CreateAsync(Service service)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_insert_service(@name, @type, @price)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;

        AddParameter(command, "@name", service.Name);
        AddParameter(command, "@type", service.Type);
        AddParameter(command, "@price", service.Price);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result) > 0;
    }

    public async Task<bool> UpdateAsync(Service service)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_update_service(@id, @name, @type, @price, @modified_by_user_id)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;

        AddParameter(command, "@id", service.Id);
        AddParameter(command, "@name", service.Name);
        AddParameter(command, "@type", service.Type);
        AddParameter(command, "@price", service.Price);
        // AddParameter(command, "@modified_by_user_id");

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return Convert.ToBoolean(result);
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_soft_delete_service(@id, @modified_by_user_id)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;

        AddParameter(command, "@id", id);
        // AddParameter(command, "@modified_by_user_id");

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return Convert.ToBoolean(result);
    }

    private Service MapReaderToModel(IDataReader reader)
    {
        return new Service
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            Type = reader.GetString(2),
            Price = reader.GetDecimal(3),
            CreatedAt = reader.GetDateTime(4),
            UpdatedAt = reader.IsDBNull(5) ? null : reader.GetDateTime(5),
            IsActive = reader.GetBoolean(6)
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
