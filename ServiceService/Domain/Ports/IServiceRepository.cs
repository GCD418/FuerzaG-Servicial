using System.Data;
using ServiceService.Domain.Entities;

namespace ServiceService.Domain.Ports;

public interface IServiceRepository
{
    public Task<IEnumerable<Service>> GetAllAsync();
    public Task<Service?> GetByIdAsync(int id);
    public Task<bool> CreateAsync(Service service, int createdByUserId);
    public Task<bool> UpdateAsync(Service service, int modifiedByUserId);
    public Task<bool> DeleteByIdAsync(int id, int modifiedByUserId);
}