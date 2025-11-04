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

    public async Task<bool> Update(UserAccount userAccount, int userId)
    {
        return await _repository.UpdateAsync(userAccount, userId);
    }

    public async Task<bool> DeleteById(int id, int userId)
    {
        return await _repository.DeleteByIdAsync(id, userId);
    }

    public async Task<UserAccount?> GetByUserName(string userName)
    {
        return await _repository.GetByUserName(userName);
    }

    public async Task<bool> IsUserNameUsed(string userName)
    {
        return await _repository.IsUserNameUsed(userName);
    }

    public async Task<bool> ChangePassword(int userId, string newPassword)
    {
        return await _repository.ChangePassword(userId, newPassword);
    }

    public string GenerateUserName(UserAccount userAccount)
    {
        var firstLetter = userAccount.Name.Split(' ')[0].ToLower()[0];
        var firstLastName = userAccount.FirstLastName.Replace(" ", "").ToLower();
        var last3 = userAccount.DocumentNumber[^3..];
        return $"{firstLetter}.{firstLastName}{last3}";
    }
}