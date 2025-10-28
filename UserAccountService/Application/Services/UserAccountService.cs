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

    public async Task<bool> Create(UserAccount owner)
    {
        return await _repository.CreateAsync(owner);
    }

    public async Task<bool> Update(UserAccount owner)
    {
        return await _repository.UpdateAsync(owner);
    }

    public async Task<bool> DeleteById(int id)
    {
        return await _repository.DeleteByIdAsync(id);
    }

    public async Task<UserAccount?> GetByUserName(string userName)
    {
        return await _repository.GetByUserName(userName);
    }

    public async Task<bool> IsUserNameUsed(string userName)
    {
        return await _repository.IsUserNameUsed(userName);
    }
    
    public string GenerateUserName(UserAccount userAccount)
    {
        var firstName = userAccount.Name.Split(' ')[0].ToLower();
        var firstLetter = firstName[0]; 
        var firstLastName = userAccount.FirstLastName.ToLower();
        var docNumber = userAccount.DocumentNumber;
        var last3 = docNumber[^3..];

        return $"{firstLetter}{firstLastName}.{last3}";
    }
    
}