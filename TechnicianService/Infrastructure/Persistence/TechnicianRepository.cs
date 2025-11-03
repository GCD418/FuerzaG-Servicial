using CommonService.Infrastructure.Connection;
using Dapper;
using TechnicianService.Domain.Entities;
using TechnicianService.Domain.Ports;

namespace TechnicianService.Infrastructure.Persistence;

public class TechnicianRepository : ITechnicianRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public TechnicianRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<Technician>> GetAllAsync()
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT * FROM fn_get_active_technicians()";
        return await connection.QueryAsync<Technician>(query);
    }

    public async Task<Technician?> GetByIdAsync(int id)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT * FROM fn_get_technician_by_id(@id)";
        return await connection.QuerySingleOrDefaultAsync<Technician>(query, new { id });
    }

    public async Task<bool> CreateAsync(Technician technician, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_insert_technician(@name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @document_extension, @address, @base_salary, @created_by_user_id)";
        var parameters = new
        {
            name = technician.Name,
            first_last_name = technician.FirstLastName,
            second_last_name = technician.SecondLastName,
            phone_number = technician.PhoneNumber,
            email = technician.Email,
            document_number = technician.DocumentNumber,
            document_extension = technician.DocumentExtension,
            address = technician.Address,
            base_salary = technician.BaseSalary,
            created_by_user_id = userId
        };
        var newId = await connection.ExecuteScalarAsync<int>(query, parameters);
        return newId > 0;
    }

    public async Task<bool> UpdateAsync(Technician technician, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        string query = "SELECT fn_update_technician(@id, @name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @document_extension, @address, @base_salary, @modified_by_user_id)";
        var parameters = new
        {
            id = technician.Id,
            name = technician.Name,
            first_last_name = technician.FirstLastName,
            second_last_name = technician.SecondLastName,
            phone_number = technician.PhoneNumber,
            email = technician.Email,
            document_number = technician.DocumentNumber,
            document_extension = technician.DocumentExtension,
            address = technician.Address,
            base_salary = technician.BaseSalary,
            modified_by_user_id = userId
        };
        return await connection.ExecuteScalarAsync<bool>(query, parameters);
    }

    public async Task<bool> DeleteByIdAsync(int id, int userId)
    {
        await using var connection = _dbConnectionFactory.CreateConnection();
        const string query = "SELECT fn_soft_delete_technician(@id, @modified_by_user_id)";
        var parameters = new
        {
            id,
            modified_by_user_id = userId
        };
        return await connection.ExecuteScalarAsync<bool>(query, parameters);
    }
}