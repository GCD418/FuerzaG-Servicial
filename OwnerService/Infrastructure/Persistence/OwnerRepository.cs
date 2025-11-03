using System.Data;
using CommonService.Infrastructure.Connection;
using Dapper;
using OwnerService.Domain.Entities;
using OwnerService.Domain.Ports;

namespace OwnerService.Infrastructure.Persistence;

public class OwnerRepository : IOwnerRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public OwnerRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<Owner>> GetAllAsync()
    {
        await using var connection = _dbConnectionFactory.CreateConnection();

        string query = "SELECT * FROM fn_get_active_owners()";
        return await connection.QueryAsync<Owner>(query);
    }

    public async Task<Owner?> GetByIdAsync(int id)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT * FROM fn_get_owner_by_id(@id)";
        return await  connection.QuerySingleOrDefaultAsync<Owner>(query, new { id = id });
    }

    public async Task<bool> CreateAsync(Owner owner, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_insert_owner(@name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @document_extension, @address, @created_by_user_id)";
        var parameters = new
        {
            name = owner.Name,
            first_last_name = owner.FirstLastname,
            second_last_name = owner.SecondLastname,
            phone_number = owner.PhoneNumber,
            email = owner.Email,
            document_number = owner.DocumentNumber,
            address = owner.Address,
            created_by_user_id = userId,
            document_extension = owner.DocumentExtension
        };
        var newId = await connection.ExecuteScalarAsync<int>(query, parameters);
        return Convert.ToInt32(newId) > 0;
    }

    public async Task<bool> UpdateAsync(Owner owner, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_update_owner(@id, @name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @document_extension, @address, @modified_by_user_id)";
        var parameters = new
        {
            id = owner.Id,
            name = owner.Name,
            first_last_name = owner.FirstLastname,
            second_last_name = owner.SecondLastname,
            phone_number = owner.PhoneNumber,
            email = owner.Email,
            document_number = owner.DocumentNumber,
            address = owner.Address,
            modified_by_user_id = userId,
            document_extension = owner.DocumentExtension
        };
        return Convert.ToBoolean(await connection.ExecuteAsync(query, parameters));
    }

    public async Task<bool> DeleteByIdAsync(int id, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_soft_delete_owner(@id, @modified_by_user_id)";
        var parameters = new
        {
            id = id,
            modified_by_user_id = userId
        };
        return Convert.ToBoolean(await connection.ExecuteScalarAsync(query, parameters));
    }

}