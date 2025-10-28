using System.Net;
using System.Net.Mail;
using CommonService.Domain.Ports;

namespace CommonService.Infrastructure;

public class MailSettings
{
    public string SmtpHost { get; set; } = "";
    public int SmtpPort { get; set; }
    public string SmtpUser { get; set; } = "";
    public string SmtpPass { get; set; } = "";
    public string FromName { get; set; } = "";
    public string FromEmail { get; set; } = "";
}

public class SmtpEmailSender : IMailSender
{
    private readonly MailSettings _settings;

    public SmtpEmailSender(IOptions<MailSettings> settings)
    {
        _settings = settings.Value;
    }

    public void SendEmail(string email, string subject, string htmlMessage)
    {
        try
        {
            using var smtpClient = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(_settings.SmtpUser, _settings.SmtpPass);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            
            mailMessage.To.Add(email);

            smtpClient.Send(mailMessage);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error al enviar el correo: {ex.Message}", ex);
        }
    }   
}