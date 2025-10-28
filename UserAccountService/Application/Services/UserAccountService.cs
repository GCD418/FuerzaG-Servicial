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

    public async Task<string> GenerateUserName(UserAccount userAccount)
    {
        if (string.IsNullOrWhiteSpace(userAccount.Name) ||
            string.IsNullOrWhiteSpace(userAccount.FirstLastName) ||
            string.IsNullOrWhiteSpace(userAccount.DocumentNumber) ||
            userAccount.DocumentNumber.Length < 3)
            return string.Empty;

        var firstName = userAccount.Name.Split(' ')[0].ToLower();
        var firstLetter = firstName[0];
        var firstLastName = userAccount.FirstLastName.ToLower();
        var last3 = userAccount.DocumentNumber[^3..];

        string baseUsername = $"{firstLetter}{firstLastName}.{last3}";
        string username = baseUsername;
        int counter = 1;

        var allUsers = await _repository.GetAllAsync();
        var existingUsernames = allUsers.Select(u => u.UserName).ToHashSet();

        while (existingUsernames.Contains(username))
        {
            username = $"{baseUsername}_{counter}";
            counter++;
        }

        return username;
    }
}
