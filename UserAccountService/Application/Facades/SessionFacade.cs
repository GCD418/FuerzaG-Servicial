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
        
        await SendEmail(userAccount.Name, userAccount.UserName, userAccount.Email, password);

        return await _userAccountService.Create(userAccount);

    }

    public async Task<bool> Login(string username, string password)
    {
        UserAccount? userAccount = await _userAccountService.GetByUserName(username);
        if (!VerifyCredentials(userAccount, password))
        {
            return false;
        }
        
        
    }

    private bool VerifyCredentials(UserAccount userAccount, string password)
    {
        if (userAccount == null || !_passwordService.VerifyPassword(password, userAccount.Password))
        {
            return false;
        }
        return true;
    }
    private async Task SendEmail(string name, string username, string email, string password)
    {
        string subject = "Bienvenido a FuerzaG";
        string body = $@"
            <h1>Hola {name}!</h1>
            <p>Tu nombre de usuario es: <strong>{username}</strong></p>
            <p>Tu contraseña es: <strong>{password}</strong></p>
            <p>Ya puedes iniciar sesión en el sistema. Recuerda cuidarla como las llaves de tu casa</p>
        ";
        await _mailSender.SendEmail(email, subject, body);
    }
    
}