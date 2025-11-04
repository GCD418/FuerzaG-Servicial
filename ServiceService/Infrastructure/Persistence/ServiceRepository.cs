using CommonService.Infrastructure.Connection;
using Dapper;
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
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT * FROM fn_get_active_services()";
        return await connection.QueryAsync<Service>(query);
    }

    public async Task<Service?> GetByIdAsync(int id)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT * FROM fn_get_service_by_id(@id)";
        return await connection.QuerySingleOrDefaultAsync<Service>(query, new { id });
    }

    public async Task<bool> CreateAsync(Service service, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_insert_service(@name, @type, @price, @description, @created_by_user_id)";
        var parameters = new
        {
            name = service.Name,
            type = service.Type,
            price = service.Price,
            description = service.Description,
            created_by_user_id = userId
        };
        var newId = await connection.ExecuteScalarAsync<int>(query, parameters);
        return newId > 0;
    }

    public async Task<bool> UpdateAsync(Service service, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_update_service(@id, @name, @type, @price, @description, @modified_by_user_id)";
        var parameters = new
        {
            id = service.Id,
            name = service.Name,
            type = service.Type,
            price = service.Price,
            description = service.Description,
            modified_by_user_id = userId
        };
        return await connection.ExecuteScalarAsync<bool>(query, parameters);
    }

    public async Task<bool> DeleteByIdAsync(int id, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_soft_delete_service(@id, @modified_by_user_id)";
        var parameters = new
        {
            id,
            modified_by_user_id = userId
        };
        return await connection.ExecuteScalarAsync<bool>(query, parameters);
    }
}
