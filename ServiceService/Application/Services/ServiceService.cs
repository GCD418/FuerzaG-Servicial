using ServiceService.Domain.Entities;
using ServiceService.Domain.Ports;

namespace ServiceService.Application.Services;

public class ServiceService
{
    private readonly IServiceRepository _repository;

    public ServiceService(IServiceRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Service>> GetAll()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Service> GetById(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> Create(Service service)
    {
        return await _repository.CreateAsync(service);
    }

    public async Task<bool> Update(Service service)
    {
        return await _repository.UpdateAsync(service);
    }

    public async Task<bool> DeleteById(int id)
    {
        return await _repository.DeleteByIdAsync(id);
    }
}
