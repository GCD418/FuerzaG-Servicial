using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using CommonService.Infrastructure.Connection;
using TechnicianService.Domain.Entities;
using TechnicianService.Domain.Ports;

namespace TechnicianService.Infrastructure.Persistence
{
    public class TechnicianRepository : ITechnicianRepository
    {
        private readonly IDbConnectionFactory _db;

        public TechnicianRepository(IDbConnectionFactory db) => _db = db;

        public async Task<IEnumerable<Technician>> GetAllAsync()
        {
            var list = new List<Technician>();
            await using var conn = _db.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM fn_get_active_technicians()";

            await conn.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
                list.Add(MapReaderToModel(r));
            return list;
        }

        public async Task<Technician?> GetByIdAsync(int id)
        {
            await using var conn = _db.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM fn_get_technician_by_id(@id)";
            AddParameter(cmd, "@id", id);

            await conn.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            return await r.ReadAsync() ? MapReaderToModel(r) : null;
        }

        public async Task<bool> CreateAsync(Technician t, int userId)
        {
            await using var conn = _db.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT fn_insert_technician(@name,@first_lastname,@second_lastname,@phone,@email,@doc,@address,@base_salary, @created_by_user_id)";

            AddParameter(cmd, "@name", t.Name);
            AddParameter(cmd, "@first_lastname", t.FirstLastName);
            AddParameter(cmd, "@second_lastname", t.SecondLastName);
            AddParameter(cmd, "@phone", t.PhoneNumber);
            AddParameter(cmd, "@email", t.Email);
            AddParameter(cmd, "@doc", t.DocumentNumber);
            AddParameter(cmd, "@address", t.Address);
            AddParameter(cmd, "@base_salary", t.BaseSalary);
            AddParameter(cmd, "@created_by_user_id", userId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> UpdateAsync(Technician t, int userId)
        {
            await using var conn = _db.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText =
                "SELECT fn_update_technician(@id,@name,@first_last_name,@second_last_name, @phone_number,@email,@document_number,@address,@base_salary,@modified_by_user_id)";

            AddParameter(cmd, "@id", t.Id);
            AddParameter(cmd, "@name", t.Name);
            AddParameter(cmd, "@first_last_name", t.FirstLastName);
            AddParameter(cmd, "@second_last_name", t.SecondLastName);
            AddParameter(cmd, "@phone_number", t.PhoneNumber);
            AddParameter(cmd, "@email", t.Email);
            AddParameter(cmd, "@document_number", t.DocumentNumber);
            AddParameter(cmd, "@address", t.Address);
            AddParameter(cmd, "@base_salary", t.BaseSalary);
            AddParameter(cmd, "@modified_by_user_id", userId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToBoolean(result);
        }

        public async Task<bool> DeleteByIdAsync(int id, int userId)
        {
            await using var conn = _db.CreateConnection();
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT fn_soft_delete_technician(@id, @modified_by_user_id)";
            AddParameter(cmd, "@id", id);
            AddParameter(cmd, "@modified_by_user_id", userId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToBoolean(result);
        }

        private static Technician MapReaderToModel(IDataReader r) => new()
        {
            Id = Convert.ToInt32(r["id"]),
            Name = r["name"] as string,
            FirstLastName = r["first_last_name"] as string,
            SecondLastName = r["second_last_name"] as string,
            PhoneNumber = r["phone_number"] is DBNull ? 0 : Convert.ToInt32(r["phone_number"]),
            Email = r["email"] as string,
            DocumentNumber = r["document_number"] as string,
            Address = r["address"] as string,
            BaseSalary = r["base_salary"] is DBNull ? null : (decimal?)Convert.ToDecimal(r["base_salary"]),
            CreatedAt = r["created_at"] is DBNull ? null : (DateTime?)Convert.ToDateTime(r["created_at"]),
            UpdatedAt = r["updated_at"] is DBNull ? null : (DateTime?)Convert.ToDateTime(r["updated_at"]),
            IsActive = r["is_active"] is DBNull ? true : (bool)r["is_active"],
            ModifiedByUserId = r["modified_by_user_id"] is DBNull
                ? null
                : (int?)Convert.ToInt32(r["modified_by_user_id"])
        };

        private static void AddParameter(IDbCommand cmd, string name, object? value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}