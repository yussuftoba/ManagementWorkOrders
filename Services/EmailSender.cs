using System.Net;
using System.Net.Mail;

namespace Services;

public class EmailSender
{
    private string hostEmail;
    private string fromEmail;
    private string password;
    private int port;
    public EmailSender(IConfiguration configuration)
    {
        fromEmail = configuration["EmailSender:FromEmail"]!;
        hostEmail = configuration["EmailSender:HostEmail"]!;
        port = int.Parse(configuration["EmailSender:Port"]!);
        password = configuration["EmailSender:Password"]!;

    }
    public void SendEmail(string subject, string toEmail, string message)
    {

        var client = new SmtpClient(hostEmail, port);
        client.EnableSsl = true;

        client.Credentials = new NetworkCredential(fromEmail, password);

        client.Send(fromEmail, toEmail, subject, message);
    }
}
