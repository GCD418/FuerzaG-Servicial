using UserAccountService.Domain.Entities;

namespace UserAccountService.Domain.Ports;

public interface IUserAccountRepository
{
    public Task<IEnumerable<UserAccount>> GetAllAsync();
    public Task<UserAccount> GetByIdAsync(int id);
    public Task<bool> CreateAsync(UserAccount userAccount);
    public Task<bool> UpdateAsync(UserAccount userAccount, int userId);
    public Task<bool> DeleteByIdAsync(int id, int userId);
    public Task<UserAccount?> GetByUserName(string userName);
    public Task<bool> IsUserNameUsed(string userName);
    public Task<bool> ChangePassword(int userId, string newPassword);
}