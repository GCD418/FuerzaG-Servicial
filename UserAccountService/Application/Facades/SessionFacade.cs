using CommonService.Domain.Ports;
using UserAccountService.Domain.Entities;
using UserAccountService.Domain.Ports;

namespace UserAccountService.Application.Facades;

public class SessionFacade
{
    private readonly Services.UserAccountService _userAccountService;
    private readonly IMailSender _mailSender;
    private readonly IPasswordService _passwordService;
    public SessionFacade(
        Services.UserAccountService userAccountService,
        IMailSender mailSender,
        IPasswordService passwordService
        )
    {
        _userAccountService = userAccountService;
        _mailSender = mailSender;
        _passwordService = passwordService;
    }

    public async Task<bool> CreateUserAccount(UserAccount userAccount)
    {
        userAccount.UserName = _userAccountService.GenerateUserName(userAccount);
        if (await _userAccountService.IsUserNameUsed(userAccount.UserName))
        {
            return false;
        }

        var password = _passwordService.GenerateRandomPassword();
        userAccount.Password = _passwordService.HashPassword(password);
        
    }
    
    
    
    
}