using System.Data;
using CommonService.Infrastructure.Connection;
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
        var owners = new List<Owner>();
        await using var connection = _dbConnectionFactory.CreateConnection();

        string query = "SELECT * FROM fn_get_active_owners()";

        await using var command = connection.CreateCommand();
        command.CommandText = query;         
        
        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            owners.Add(MapReaderToModel(reader));

        return owners;
    }

    public async Task<Owner?> GetByIdAsync(int id)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT * FROM fn_get_owner_by_id(@id)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;   
        AddParameter(command, "@id", id);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapReaderToModel(reader) : null;
    }

    public async Task<bool> CreateAsync(Owner owner, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_insert_owner(@name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @address, @created_by_user_id, @document_extension)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;             

        AddParameter(command, "@name",             owner.Name);
        AddParameter(command, "@first_last_name",  owner.FirstLastname);
        AddParameter(command, "@second_last_name", owner.SecondLastname);
        AddParameter(command, "@phone_number",     owner.PhoneNumber);
        AddParameter(command, "@email",            owner.Email);
        AddParameter(command, "@document_number",  owner.Ci);
        AddParameter(command, "@address",          owner.Address);
        AddParameter(command, "@created_by_user_id", userId);
        AddParameter(command, "@document_extension", owner.DocumentExtension);

        await connection.OpenAsync();
        var idObj = await command.ExecuteScalarAsync();        
        return Convert.ToInt32(idObj) > 0;
    }

    public async Task<bool> UpdateAsync(Owner owner, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_update_owner(@id, @name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @address, @modified_by_user_id, @document_extension)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;           

        AddParameter(command, "@name",             owner.Name);
        AddParameter(command, "@first_last_name",  owner.FirstLastname);
        AddParameter(command, "@second_last_name", owner.SecondLastname);
        AddParameter(command, "@phone_number",     owner.PhoneNumber);
        AddParameter(command, "@email",            owner.Email);
        AddParameter(command, "@document_number",  owner.Ci);
        AddParameter(command, "@address",          owner.Address);
        AddParameter(command, "@modified_by_user_id", userId);
        AddParameter(command, "@id",               owner.Id);
        AddParameter(command, "@document_extension", owner.DocumentExtension);

        await connection.OpenAsync();
        return Convert.ToBoolean(await command.ExecuteScalarAsync());
    }

    public async Task<bool> DeleteByIdAsync(int id, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_soft_delete_owner(@id, @modified_by_user_id)";
        await using var command = connection.CreateCommand();
        command.CommandText = query;                  
        AddParameter(command, "@id", id);
        AddParameter(command, "@modified_by_user_id", userId);

        await connection.OpenAsync();
        return Convert.ToBoolean(await command.ExecuteScalarAsync());
    }

    private Owner MapReaderToModel(IDataReader reader)
    {
        return new Owner
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            FirstLastname = reader.GetString(2),
            SecondLastname = reader.IsDBNull(3) ? null : reader.GetString(3),
            PhoneNumber = reader.GetInt32(4),
            Email = reader.GetString(5),
            Ci = reader.GetString(6),
            Address = reader.GetString(7),
            CreatedAt = reader.GetDateTime(8),
            UpdatedAt = reader.IsDBNull(9) ? null : reader.GetDateTime(9),
            IsActive = reader.GetBoolean(10),
            DocumentExtension = reader.GetString(13)
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