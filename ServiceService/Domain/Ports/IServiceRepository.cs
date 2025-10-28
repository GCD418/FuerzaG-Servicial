using ServiceService.Domain.Entities;

namespace ServiceService.Domain.Ports;

public interface IServiceRepository
{
    Task<IEnumerable<Service>> GetAllAsync();
    Task<Service?> GetByIdAsync(int id);
    Task<bool> CreateAsync(Service service);
    Task<bool> UpdateAsync(Service service);
    Task<bool> DeleteByIdAsync(int id);
}
