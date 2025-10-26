using OwnerService.Domain.Entities;
using OwnerService.Domain.Ports;

namespace OwnerService.Application.Services;

public class OwnerService
{
    private readonly IOwnerRepository _repository;
    
    public OwnerService(IOwnerRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Owner>> GetAll()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Owner> GetById(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> Create(Owner owner)
    {
        return await _repository.CreateAsync(owner);
    }

    public async Task<bool> Update(Owner owner)
    {
        return await _repository.UpdateAsync(owner);
    }

    public async Task<bool> DeleteById(int id)
    {
        return await _repository.DeleteByIdAsync(id);
    }
}