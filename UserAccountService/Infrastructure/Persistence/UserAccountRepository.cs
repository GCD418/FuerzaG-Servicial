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

        string query = "SELECT * FROM fn_get_active_accounts()";

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
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query =
            "SELECT fn_insert_account(@name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @user_name, @password, @role)";

        await using var command = connection.CreateCommand();
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

    public async Task<bool> UpdateAsync(UserAccount userAccount, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();

        // No se actualiza password
        string query =
            "SELECT fn_update_account_no_password(@id, @name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @role, @modified_by_user_id)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;

        AddParameter(command, "@id", userAccount.Id);
        AddParameter(command, "@name", userAccount.Name);
        AddParameter(command, "@first_last_name", userAccount.FirstLastName);
        AddParameter(command, "@second_last_name", userAccount.SecondLastName);
        AddParameter(command, "@phone_number", userAccount.PhoneNumber);
        AddParameter(command, "@email", userAccount.Email);
        AddParameter(command, "@document_number", userAccount.DocumentNumber);
        AddParameter(command, "@role", userAccount.Role);
        AddParameter(command, "@modified_by_user_id", userId);

        await connection.OpenAsync();
        return Convert.ToBoolean(await command.ExecuteScalarAsync());
    }

    public async Task<bool> DeleteByIdAsync(int id, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_soft_delete_account(@id, @modified_by_user_id)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;
        AddParameter(command, "@id", id);
        AddParameter(command, "@modified_by_user_id", userId);

        await connection.OpenAsync();
        return Convert.ToBoolean(await command.ExecuteScalarAsync());
    }

    public async Task<bool> UpdateIsFirstLoginAsync(int userId, bool isFirstLogin)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "UPDATE user_account SET is_first_login = @is_first_login WHERE id = @id";

        await using var command = connection.CreateCommand();
        command.CommandText = query;
        AddParameter(command, "@is_first_login", isFirstLogin);
        AddParameter(command, "@id", userId);

        await connection.OpenAsync();
        return await command.ExecuteNonQueryAsync() > 0;
    }
    public async Task<UserAccount?> GetByUserName(string userName)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT * FROM fn_get_account_by_username(@user_name)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;
        AddParameter(command, "@user_name", userName);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();
        return await reader.ReadAsync() ? MapReaderToModel(reader) : null;
    }

    public async Task<bool> IsUserNameUsed(string userName)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_account_exists_by_username(@user_name)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;
        AddParameter(command, "@user_name", userName);

        await connection.OpenAsync();
        return Convert.ToBoolean(await command.ExecuteScalarAsync());
    }

    private UserAccount MapReaderToModel(IDataReader reader)
    {
        return new UserAccount
        {
            Id = reader.GetInt32(reader.GetOrdinal("id")),
            Name = reader.GetString(reader.GetOrdinal("name")),
            FirstLastName = reader.GetString(reader.GetOrdinal("first_last_name")),
            SecondLastName = reader.IsDBNull(reader.GetOrdinal("second_last_name"))
                ? null
                : reader.GetString(reader.GetOrdinal("second_last_name")),
            PhoneNumber = reader.IsDBNull(reader.GetOrdinal("phone_number"))
                ? 0
                : reader.GetInt32(reader.GetOrdinal("phone_number")),
            Email = reader.IsDBNull(reader.GetOrdinal("email"))
                ? string.Empty
                : reader.GetString(reader.GetOrdinal("email")),
            DocumentNumber = reader.IsDBNull(reader.GetOrdinal("document_number"))
                ? string.Empty
                : reader.GetString(reader.GetOrdinal("document_number")),
            UserName = reader.GetString(reader.GetOrdinal("user_name")),
            Password = reader.IsDBNull(reader.GetOrdinal("password"))
                ? null
                : reader.GetString(
                    reader.GetOrdinal("password")),
            Role = reader.GetString(reader.GetOrdinal("role")),
            CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("updated_at"))
                ? null
                : reader.GetDateTime(reader.GetOrdinal("updated_at")),
            IsActive = reader.GetBoolean(reader.GetOrdinal("is_active")),
            ModifiedByUserId = reader.IsDBNull(reader.GetOrdinal("modified_by_user_id"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("modified_by_user_id")),
            IsFirstLogin = reader.GetBoolean(reader.GetOrdinal("is_first_login"))
        };
    }

    public async Task<bool> ChangePassword(int userId, string newPassword)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();

        string query =
            "SELECT fn_update_password_account(@id, @password, @modified_by_user_id)";

        await using var command = connection.CreateCommand();
        command.CommandText = query;

        AddParameter(command, "@id", userId);
        AddParameter(command, "@password", newPassword);
        AddParameter(command, "@modified_by_user_id", userId);

        await connection.OpenAsync();
        return Convert.ToBoolean(await command.ExecuteScalarAsync());       
    }

    private void AddParameter(IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}