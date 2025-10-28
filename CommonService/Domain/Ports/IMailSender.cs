namespace CommonService.Domain.Ports;

public interface IMailSender
{
    Task SendEmail(string email, string subject, string message);
}