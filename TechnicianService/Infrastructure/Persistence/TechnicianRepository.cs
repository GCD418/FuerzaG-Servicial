using System.Data;
using FuerzaG.Domain.Ports;
using FuerzaG.Infrastructure.Connection;
using FuerzaG.Models;

namespace FuerzaG.Infrastructure.Persistence
{
    public class TechnicianRepository : IRepository<Technician>
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public TechnicianRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public List<Technician> GetAll()
        {
            const string sql = "SELECT * FROM fn_get_active_technicians()";

            using var conn = _connectionFactory.CreateConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandType = CommandType.Text;

            using var reader = cmd.ExecuteReader();
            var list = new List<Technician>();
            while (reader.Read())
                list.Add(MapReaderToModel(reader));
            return list;
        }

        public Technician GetById(int id)
        {
            const string sql = "SELECT * FROM fn_get_technician_by_id(@id)";

            using var conn = _connectionFactory.CreateConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            AddParam(cmd, "@id", id);

            using var reader = cmd.ExecuteReader();
            return reader.Read() ? MapReaderToModel(reader) : null;
        }

        public int Create(Technician t)
        {
            const string sql = "SELECT fn_insert_technician(@name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @address, @base_salary)";

            using var conn = _connectionFactory.CreateConnection();
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            AddParam(cmd, "@name", t.Name);
            AddParam(cmd, "@first_last_name", t.FirstLastName);
            AddParam(cmd, "@second_last_name", t.SecondLastName);
            AddParam(cmd, "@phone_number", t.PhoneNumber);
            AddParam(cmd, "@email", t.Email);
            AddParam(cmd, "@document_number", t.DocumentNumber);
            AddParam(cmd, "@address", t.Address);
            AddParam(cmd, "@base_salary", t.BaseSalary);

            var idObj = cmd.ExecuteScalar();
            return Convert.ToInt32(idObj);
        }

        public bool Update(Technician t)
        {
            const string sql = "SELECT fn_update_technician(@id, @name, @first_last_name, @second_last_name, @phone_number, @email, @document_number, @address, @base_salary, @modified_by_user_id)";

            using var conn = _connectionFactory.CreateConnection();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;

            AddParam(cmd, "@id", t.Id);
            AddParam(cmd, "@name", t.Name);
            AddParam(cmd, "@first_last_name", t.FirstLastName);
            AddParam(cmd, "@second_last_name", t.SecondLastName);
            AddParam(cmd, "@phone_number", t.PhoneNumber);
            AddParam(cmd, "@email", t.Email);
            AddParam(cmd, "@document_number", t.DocumentNumber);
            AddParam(cmd, "@address", t.Address);
            AddParam(cmd, "@base_salary", t.BaseSalary);
            AddParam(cmd, "@modified_by_user_id", t.ModifiedByUserId);
            conn.Open();

            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        public bool DeleteById(int id)
        {
            using var connection = _connectionFactory.CreateConnection();

            const string sql = "SELECT fn_soft_delete_technician(@id, @modified_by_user_id)";
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            AddParam(command, "@id", id);
            AddParam(command, "@modified_by_user_id", 9999); //TODO implement real ids
            connection.Open();
            return Convert.ToBoolean(command.ExecuteScalar());
        }

        public Technician MapReaderToModel(IDataReader reader)
        {
            var r = (IDataRecord)reader;
            return new Technician
            {
                Id = r.GetInt32(r.GetOrdinal("id")),
                Name = r["name"] as string,
                FirstLastName = r["first_last_name"] as string,
                SecondLastName = r["second_last_name"] as string,
                PhoneNumber = r.GetInt32(r.GetOrdinal("phone_number")),
                Email = r["email"] as string,
                DocumentNumber = r["document_number"] as string,
                Address = r["address"] as string,
                BaseSalary = r["base_salary"] is DBNull ? null : (decimal?)r.GetDecimal(r.GetOrdinal("base_salary")),
                CreatedAt = r["created_at"] is DBNull ? null : (DateTime?)r.GetDateTime(r.GetOrdinal("created_at")),
                UpdatedAt = r["updated_at"] is DBNull ? null : (DateTime?)r.GetDateTime(r.GetOrdinal("updated_at")),
                IsActive = r["is_active"] is DBNull ? true : (bool)r["is_active"],
                ModifiedByUserId = r["modified_by_user_id"] is DBNull ? null : (int?)Convert.ToInt32(r["modified_by_user_id"])
            };
        }

        private static void AddParam(IDbCommand cmd, string name, object? value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}
