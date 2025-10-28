using System.Data;
using OwnerService.Domain.Entities;

namespace OwnerService.Domain.Ports;

public interface IOwnerRepository
{
    public Task<IEnumerable<Owner>> GetAllAsync();
    public Task<Owner> GetByIdAsync(int id);
    public Task<bool> CreateAsync(Owner owner);
    public Task<bool> UpdateAsync(Owner owner, int userId);
    public Task<bool> DeleteByIdAsync(int id, int userId);
}