using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;

namespace UserAccountService.Application.Services;

public class UserAccountService
{
    private readonly IUserAccountRepository _repository;
    
    public UserAccountService(IUserAccountRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<UserAccount>> GetAll()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<UserAccount> GetById(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<bool> Create(UserAccount userAccount)
    {
        return await _repository.CreateAsync(userAccount);
    }

    public async Task<bool> Update(UserAccount userAccount)
    {
        return await _repository.UpdateAsync(userAccount);
    }

    public async Task<bool> DeleteById(int id)
    {
        return await _repository.DeleteByIdAsync(id);
    }
}