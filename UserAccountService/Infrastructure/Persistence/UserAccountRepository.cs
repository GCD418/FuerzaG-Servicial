using CommonService.Infrastructure.Connection;
using Dapper;
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
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT * FROM fn_get_active_accounts()";
        return await connection.QueryAsync<UserAccount>(query);
    }

    public async Task<UserAccount?> GetByIdAsync(int id)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT * FROM fn_get_account_by_id(@id)";
        return await connection.QuerySingleOrDefaultAsync<UserAccount>(query, new { id });
    }

    public async Task<bool> CreateAsync(UserAccount userAccount)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_insert_account(@name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @document_extension, @user_name, @password, @role, @created_by_user_id)";
        var parameters = new
        {
            name = userAccount.Name,
            first_last_name = userAccount.FirstLastName,
            second_last_name = userAccount.SecondLastName,
            phone_number = userAccount.PhoneNumber,
            email = userAccount.Email,
            document_number = userAccount.DocumentNumber,
            document_extension = userAccount.DocumentExtension,
            user_name = userAccount.UserName,
            password = userAccount.Password,
            role = userAccount.Role,
            created_by_user_id = userAccount.ModifiedByUserId
        };
        var newId = await connection.ExecuteScalarAsync<int>(query, parameters);
        return newId > 0;
    }

    public async Task<bool> UpdateAsync(UserAccount userAccount, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_update_account_no_password(@id, @name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @document_extension, @role, @modified_by_user_id)";
        var parameters = new
        {
            id = userAccount.Id,
            name = userAccount.Name,
            first_last_name = userAccount.FirstLastName,
            second_last_name = userAccount.SecondLastName,
            phone_number = userAccount.PhoneNumber,
            email = userAccount.Email,
            document_number = userAccount.DocumentNumber,
            document_extension = userAccount.DocumentExtension,
            role = userAccount.Role,
            modified_by_user_id = userId
        };
        return await connection.ExecuteScalarAsync<bool>(query, parameters);
    }

    public async Task<bool> DeleteByIdAsync(int id, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_soft_delete_account(@id, @modified_by_user_id)";
        var parameters = new
        {
            id,
            modified_by_user_id = userId
        };
        return await connection.ExecuteScalarAsync<bool>(query, parameters);
    }

    public async Task<UserAccount?> GetByUserName(string userName)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT * FROM fn_get_account_by_username(@user_name)";
        return await connection.QuerySingleOrDefaultAsync<UserAccount>(query, new { user_name = userName });
    }

    public async Task<bool> IsUserNameUsed(string userName)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_account_exists_by_username(@user_name)";
        return await connection.ExecuteScalarAsync<bool>(query, new { user_name = userName });
    }

    public async Task<bool> ChangePassword(int userId, string newPassword)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_update_password_account(@id, @password, @modified_by_user_id)";
        var parameters = new
        {
            id = userId,
            password = newPassword,
            modified_by_user_id = userId
        };
        return await connection.ExecuteScalarAsync<bool>(query, parameters);
    }
}