using System.Data;
using CommonService.Infrastructure.Connection;
using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;

namespace UserAccountService.Infrastructure.Persistence;

public class UserAccountRepository : IUserAccountRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public UserAccountRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<UserAccount>> GetAllAsync()
    {
        var userAccounts = new List<UserAccount>();
        await using var connection = _dbConnectionFactory.CreateConnection();

        string query = "SELECT * FROM fn_get_active_userAccounts()";

        await using var command = connection.CreateCommand();
        command.CommandText = query;         
        
        await connection.OpenAsync();

        await using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
            userAccounts.Add(MapReaderToModel(reader));

        return userAccounts;
    }

    public async Task<UserAccount?> GetByIdAsync(int id)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT * FROM fn_get_account_by_id(@id)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;   
        AddParameter(command, "@id", id);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapReaderToModel(reader) : null;
    }

    public async Task<bool> CreateAsync(UserAccount userAccount)
    {
        using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_insert_account(@name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @user_name, @password, @role)";

        using var command = connection.CreateCommand();
        command.CommandText = query;

        AddParameter(command, "@name", userAccount.Name);
        AddParameter(command, "@first_last_name", userAccount.FirstLastName);
        AddParameter(command, "@second_last_name", userAccount.SecondLastName);
        AddParameter(command, "@phone_number", userAccount.PhoneNumber);
        AddParameter(command, "@email", userAccount.Email);
        AddParameter(command, "@document_number", userAccount.DocumentNumber);
        AddParameter(command, "@user_name", userAccount.UserName);
        AddParameter(command, "@password", userAccount.Password);
        AddParameter(command, "@role", userAccount.Role);

        await connection.OpenAsync();
        var idObj = await command.ExecuteScalarAsync();        
        return Convert.ToInt32(idObj) > 0;
    }

    public async Task<bool> UpdateAsync(UserAccount userAccount)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_update_account(@id, @name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @password, @role, @modified_by_user_id)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;           

        AddParameter(command, "@name", userAccount.Name);
        AddParameter(command, "@first_last_name", userAccount.FirstLastName);
        AddParameter(command, "@second_last_name", userAccount.SecondLastName);
        AddParameter(command, "@phone_number", userAccount.PhoneNumber);
        AddParameter(command, "@email", userAccount.Email);
        AddParameter(command, "@document_number", userAccount.DocumentNumber);
        AddParameter(command, "@role", userAccount.Role);
        AddParameter(command, "@passowrd", userAccount.Password);
        AddParameter(command, "@modified_by_user_id", 9999);
        AddParameter(command, "@id", userAccount.Id);

        await connection.OpenAsync();
        return Convert.ToBoolean(await command.ExecuteScalarAsync());
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_soft_delete_account(@id, @modified_by_user_id)";
        await using var command = connection.CreateCommand();
        command.CommandText = query;                  
        AddParameter(command, "@id", id);
        AddParameter(command, "@modified_by_user_id", 8888); 

        await connection.OpenAsync();
        return Convert.ToBoolean(await command.ExecuteScalarAsync());
    }

    private UserAccount MapReaderToModel(IDataReader reader)
    {
        return new UserAccount
        {
            Id = reader.GetInt32(0),
            Name = reader.GetString(1),
            FirstLastName = reader.GetString(2),
            SecondLastName = reader.IsDBNull(3) ? null : reader.GetString(3),
            PhoneNumber = reader.GetInt32(4),
            Email = reader.GetString(5),
            DocumentNumber = reader.GetString(6),
            UserName = reader.GetString(7),
            Password = reader.GetString(8),
            Role = reader.GetString(9),
            CreatedAt = reader.GetDateTime(10),
            UpdatedAt = reader.IsDBNull(11) ? null : reader.GetDateTime(9),
            IsActive = reader.GetBoolean(12)
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