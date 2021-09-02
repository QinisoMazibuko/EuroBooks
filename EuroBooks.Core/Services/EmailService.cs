using EuroBooks.Application.Common.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EuroBooks.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        public string SMTPServer { get; set; }
        public string SMTPUserName { get; set; }
        public string SMTPPassword { get; set; }
        public string SMTPFromEmail { get; set; }
        public string SMTPFromName { get; set; }
        public int SMTPPort { get; set; }

        private readonly IApplicationDbContext context;

        public EmailService(IApplicationDbContext context)
        {
            this.context = context;

            SMTPServer = context.ApplicationVariables.FirstOrDefault(x => x.Key == "SMTPServer").Value;
            SMTPUserName = context.ApplicationVariables.FirstOrDefault(x => x.Key == "SMTPUser").Value;
            SMTPPassword = context.ApplicationVariables.FirstOrDefault(x => x.Key == "SMTPPassword").Value;
            SMTPFromEmail = context.ApplicationVariables.FirstOrDefault(x => x.Key == "SMTPFromEmail").Value;
            SMTPFromName = context.ApplicationVariables.FirstOrDefault(x => x.Key == "SMTPFromName").Value;
            int port;
            int.TryParse(context.ApplicationVariables.FirstOrDefault(x => x.Key == "SMTPPort").Value, out port);
            SMTPPort = port;
        }

        public async Task SendEmail(string To, string Subject, string HTMLMessage)
        {
            try
            {

                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(SMTPServer, SMTPPort);

                mail.From = new MailAddress(SMTPFromEmail, SMTPFromName);
                mail.To.Add(To);
                mail.Subject = Subject;
                mail.Body = HTMLMessage;
                mail.IsBodyHtml = true;

                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(SMTPUserName, SMTPPassword);
                SmtpServer.EnableSsl = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                await SmtpServer.SendMailAsync(mail);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
