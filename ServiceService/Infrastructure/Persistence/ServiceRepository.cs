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

    public async Task<bool> CreateAsync(Service service, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_insert_service(@name, @type, @price, @description, @created_by_user_id)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;

        AddParameter(command, "@name", service.Name);
        AddParameter(command, "@type", service.Type);
        AddParameter(command, "@price", service.Price);
        AddParameter(command, "@description", service.Description);
        AddParameter(command, "@created_by_user_id", userId);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return Convert.ToInt32(result) > 0;
        
    }

    public async Task<bool> UpdateAsync(Service service, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_update_service(@id, @name, @type, @price, @description, @modified_by_user_id)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;

        AddParameter(command, "@id", service.Id);
        AddParameter(command, "@name", service.Name);
        AddParameter(command, "@type", service.Type);
        AddParameter(command, "@price", service.Price);
        AddParameter(command, "@description", service.Description);
        AddParameter(command, "@modified_by_user_id", userId);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return Convert.ToBoolean(result);
    }

    public async Task<bool> DeleteByIdAsync(int id, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_soft_delete_service(@id, @modified_by_user_id)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;

        AddParameter(command, "@id", id);
        AddParameter(command, "@modified_by_user_id", userId);

        await connection.OpenAsync();
        var result = await command.ExecuteScalarAsync();

        return Convert.ToBoolean(result);
    }

    private Service MapReaderToModel(IDataReader reader)
    {
        return new Service
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            Type = reader.GetString(reader.GetOrdinal("type")),
            Price = reader.GetDecimal(reader.GetOrdinal("price")),
            Description = reader.GetString(reader.GetOrdinal("description")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("updated_at")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
            ModifiedByUserId = reader.IsDBNull(reader.GetOrdinal("modified_by_user_id"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("modified_by_user_id"))
        };
    }

    private static void AddParameter(IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}
