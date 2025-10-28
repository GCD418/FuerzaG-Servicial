using OwnerService.Domain.Entities;
using OwnerService.Domain.Ports;
using UserAccountService.Domain.Ports;

namespace OwnerService.Application.Services;

public class OwnerService
{
    private readonly IOwnerRepository _repository;
    private readonly ISessionManager _sessionManager;
    
    public OwnerService(IOwnerRepository repository, ISessionManager sessionManager)
    {
        _repository = repository;
        _sessionManager = sessionManager;
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
        return await _repository.UpdateAsync(owner, _sessionManager.UserId ?? 9999);
    }

    public async Task<bool> DeleteById(int id)
    {
        return await _repository.DeleteByIdAsync(id,  _sessionManager.UserId ?? 9999);
    }
}