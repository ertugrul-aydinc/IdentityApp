using AspNetCoreIdentity.App.Core.OptionsModel;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace AspNetCoreIdentity.App.Service.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailInfos _emailInfos;

        public EmailService(IOptions<EmailInfos> options)
        {
            _emailInfos = options.Value;
        }

        public async Task SendResetPasswordEmailAsync(string resetPasswordEmailLink, string to)
        {
            var smtpClient = new SmtpClient();

            smtpClient.Host = _emailInfos.Host;
            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Port = 587;
            smtpClient.Credentials = new NetworkCredential(_emailInfos.Username, _emailInfos.Password);
            smtpClient.EnableSsl = true;

            var mailMessage = new MailMessage();

            mailMessage.From = new MailAddress(_emailInfos.Username);
            mailMessage.To.Add(to);

            mailMessage.Subject = "Localhost password reset link";
            mailMessage.Body = @$"<h4>Click on the link to reset your password.</h4>
                        <p>
                            <a href='{resetPasswordEmailLink}'>Reset password link.</a>
                        </p>";
            mailMessage.IsBodyHtml = true;

            await smtpClient.SendMailAsync(mailMessage);
            
        }
    }
}
