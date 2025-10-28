using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;

namespace UserAccountService.Application.Services;

public class UserAccountService
{
    private readonly IUserAccountRepository _repository;
    private readonly ISessionManager _sessionManager;
    
    public UserAccountService(IUserAccountRepository repository, ISessionManager sessionManager)
    {
        _repository = repository;
        _sessionManager = sessionManager;
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
        return await _repository.UpdateAsync(owner, _sessionManager.UserId ?? 9999);
    }

    public async Task<bool> DeleteById(int id)
    {
        return await _repository.DeleteByIdAsync(id, _sessionManager.UserId ?? 9999);
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