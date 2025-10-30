using System.Data;
using CommonService.Domain.Entities;
using CommonService.Domain.Ports;
using CommonService.Infrastructure.Connection;

namespace CommonService.Infrastructure.Reports.Repositories;

public class VehicleReportRepository : IVehicleReportRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public VehicleReportRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<VehicleReportDto>> GetVehiclesByYearRangeAsync(int yearFrom, int yearTo)
    {
        var vehicles = new List<VehicleReportDto>();
        await using var connection = _dbConnectionFactory.CreateConnection();

        const string query = @"
            SELECT 
                CONCAT(o.name, ' ', o.first_last_name, COALESCE(' ' || o.second_last_name, '')) as owner_full_name,
                v.plate,
                b.name as brand_name,
                m.name as model_name,
                v.year,
                COALESCE(v.color, 'N/A') as color,
                COALESCE(v.type, 'N/A') as vehicle_type
            FROM vehicle v
            INNER JOIN owner o ON v.owner_id = o.id
            INNER JOIN model m ON v.model_id = m.id
            INNER JOIN brand b ON m.brand_id = b.id
            WHERE v.is_active = true
                AND v.year >= @year_from
                AND v.year <= @year_to
            ORDER BY v.year DESC, b.name, m.name";

        await using var command = connection.CreateCommand();
        command.CommandText = query;
        AddParameter(command, "@year_from", yearFrom);
        AddParameter(command, "@year_to", yearTo);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            vehicles.Add(new VehicleReportDto
            {
                OwnerFullName = reader.GetString(0),
                Plate = reader.IsDBNull(1) ? "N/A" : reader.GetString(1),
                BrandName = reader.GetString(2),
                ModelName = reader.GetString(3),
                Year = reader.GetInt16(4),
                Color = reader.GetString(5),
                VehicleType = reader.GetString(6)
            });
        }

        return vehicles;
    }

    private static void AddParameter(IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}
