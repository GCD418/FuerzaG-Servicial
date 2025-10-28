using CommonService.Domain.Ports;
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
    
    
}