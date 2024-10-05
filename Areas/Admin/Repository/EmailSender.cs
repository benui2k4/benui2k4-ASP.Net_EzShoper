using System.Net.Mail;
using System.Net;

namespace ASP.Net_EzShoper.Areas.Admin.Repository
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true, //bật bảo mật
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("sondz2k4g@gmail.com", "asjyfgjwcloyaref")
            };

            return client.SendMailAsync(
                new MailMessage(from: "sondz2k4g@gmail.com",
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
