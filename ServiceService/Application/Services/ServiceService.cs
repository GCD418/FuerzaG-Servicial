using ServiceService.Domain.Entities;
using ServiceService.Domain.Ports;
using UserAccountService.Domain.Ports;

namespace ServiceService.Application.Services;

public class ServiceService
{
    private readonly IServiceRepository _repository;
    private readonly ISessionManager _sessionManager;
    
    public ServiceService(IServiceRepository repository, ISessionManager sessionManager)
    {
        _repository = repository;
        _sessionManager = sessionManager;
    }

    public async Task<IEnumerable<Service>> GetAll()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Service?> GetById(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> Create(Service service)
    {
        return await _repository.CreateAsync(service);
    }

    public async Task<bool> Update(Service service)
    {
        return await _repository.UpdateAsync(service, _sessionManager.UserId ?? 9999);
    }

    public async Task<bool> DeleteById(int id)
    {
        return await _repository.DeleteByIdAsync(id, _sessionManager.UserId ?? 9999);
    }
}