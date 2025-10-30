using System.Data;
using CommonService.Domain.Entities;
using CommonService.Domain.Ports;
using CommonService.Infrastructure.Connection;

namespace CommonService.Infrastructure.Reports.Repositories;

public class BrandReportRepository : IBrandReportRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public BrandReportRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<IEnumerable<BrandStatisticsDto>> GetBrandStatisticsByVehicleCountAsync(int minCount, int maxCount)
    {
        var statistics = new List<BrandStatisticsDto>();
        await using var connection = _dbConnectionFactory.CreateConnection();

        const string query = @"
            WITH brand_counts AS (
                SELECT 
                    b.name as brand_name,
                    COUNT(v.id) as vehicle_count
                FROM brand b
                INNER JOIN model m ON b.id = m.brand_id
                INNER JOIN vehicle v ON m.id = v.model_id
                WHERE v.is_active = true
                GROUP BY b.id, b.name
                HAVING COUNT(v.id) BETWEEN @min_count AND @max_count
            ),
            total_vehicles AS (
                SELECT SUM(vehicle_count) as total
                FROM brand_counts
            )
            SELECT 
                bc.brand_name,
                bc.vehicle_count,
                ROUND((bc.vehicle_count::numeric / NULLIF(tv.total, 0) * 100), 2) as percentage
            FROM brand_counts bc
            CROSS JOIN total_vehicles tv
            ORDER BY bc.vehicle_count DESC, bc.brand_name";

        await using var command = connection.CreateCommand();
        command.CommandText = query;
        AddParameter(command, "@min_count", minCount);
        AddParameter(command, "@max_count", maxCount);

        await connection.OpenAsync();
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            statistics.Add(new BrandStatisticsDto
            {
                Marca = reader.GetString(0),
                CantidadDeVehículos = reader.GetInt32(1),
                Porcentaje = reader.GetDecimal(2)
            });
        }

        return statistics;
    }

    private static void AddParameter(IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}
