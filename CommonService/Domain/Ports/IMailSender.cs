namespace CommonService.Domain.Ports;

public interface IMailSender
{
    void SendEmail(string email, string subject, string message);
}