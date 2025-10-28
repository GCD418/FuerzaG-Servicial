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
            await using var cmd  = conn.CreateCommand();
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
            await using var cmd  = conn.CreateCommand();
            cmd.CommandText = "SELECT * FROM fn_get_technician_by_id(@id)"; 
            Add(cmd, "@id", id);

            await conn.OpenAsync();
            await using var r = await cmd.ExecuteReaderAsync();
            return await r.ReadAsync() ? MapReaderToModel(r) : null;
        }

        public async Task<bool> CreateAsync(Technician t)
        {
            await using var conn = _db.CreateConnection();
            await using var cmd  = conn.CreateCommand();
            cmd.CommandText =
                "SELECT fn_insert_technician(@name,@first_lastname,@second_lastname,@phone,@email,@doc,@address,@base_salary,@created_at,@updated_at,@is_active,@modified_by)";

            Add(cmd, "@name",            t.Name);
            Add(cmd, "@first_lastname",  t.FirstLastName);
            Add(cmd, "@second_lastname", t.SecondLastName);
            Add(cmd, "@phone",           t.PhoneNumber);
            Add(cmd, "@email",           t.Email);
            Add(cmd, "@doc",             t.DocumentNumber);
            Add(cmd, "@address",         t.Address);
            Add(cmd, "@base_salary",     t.BaseSalary);
            Add(cmd, "@created_at",      t.CreatedAt);
            Add(cmd, "@updated_at",      t.UpdatedAt);
            Add(cmd, "@is_active",       t.IsActive);
            Add(cmd, "@modified_by",     t.ModifiedByUserId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result) > 0;
        }

        public async Task<bool> UpdateAsync(Technician t)
        {
            await using var conn = _db.CreateConnection();
            await using var cmd  = conn.CreateCommand();
            cmd.CommandText =
                "SELECT fn_update_technician(@id,@name,@first_lastname,@second_lastname,@phone,@email,@doc,@address,@base_salary,@updated_at,@is_active,@modified_by)";

            Add(cmd, "@id",              t.Id);
            Add(cmd, "@name",            t.Name);
            Add(cmd, "@first_lastname",  t.FirstLastName);
            Add(cmd, "@second_lastname", t.SecondLastName);
            Add(cmd, "@phone",           t.PhoneNumber);
            Add(cmd, "@email",           t.Email);
            Add(cmd, "@doc",             t.DocumentNumber);
            Add(cmd, "@address",         t.Address);
            Add(cmd, "@base_salary",     t.BaseSalary);
            Add(cmd, "@updated_at",      t.UpdatedAt);
            Add(cmd, "@is_active",       t.IsActive);
            Add(cmd, "@modified_by",     t.ModifiedByUserId);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToBoolean(result);
        }

        public async Task<bool> DeleteByIdAsync(int id)
        {
            await using var conn = _db.CreateConnection();
            await using var cmd  = conn.CreateCommand();
            cmd.CommandText = "SELECT fn_soft_delete_technician(@id)";
            Add(cmd, "@id", id);

            await conn.OpenAsync();
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToBoolean(result);
        }

        private static Technician MapReaderToModel(IDataReader r) => new()
        {
            Id               = Convert.ToInt32(r["id"]),
            Name             = r["name"] as string,
            FirstLastName    = r["first_last_name"] as string,
            SecondLastName   = r["second_last_name"] as string,
            PhoneNumber      = r["phone_number"] is DBNull ? 0 : Convert.ToInt32(r["phone_number"]),
            Email            = r["email"] as string,
            DocumentNumber   = r["document_number"] as string,
            Address          = r["address"] as string,
            BaseSalary       = r["base_salary"] is DBNull ? null : (decimal?)Convert.ToDecimal(r["base_salary"]),
            CreatedAt        = r["created_at"] is DBNull ? null : (DateTime?)Convert.ToDateTime(r["created_at"]),
            UpdatedAt        = r["updated_at"] is DBNull ? null : (DateTime?)Convert.ToDateTime(r["updated_at"]),
            IsActive         = r["is_active"] is DBNull ? true : (bool)r["is_active"],
            ModifiedByUserId = r["modified_by_user_id"] is DBNull ? null : (int?)Convert.ToInt32(r["modified_by_user_id"])
        };

        private static void Add(IDbCommand cmd, string name, object? value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}
