namespace UserAccountService.Application.Facades;

public class SessionFacade
{
    private readonly Services.UserAccountService _userAccountService;
    private readonly IMailSender _mailSender;
    
    public SessionFacade(
        Services.UserAccountService userAccountService
        )
    {
        _userAccountService = userAccountService;
    }
    
    
}