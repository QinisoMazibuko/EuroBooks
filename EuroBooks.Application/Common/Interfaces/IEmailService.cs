using System.Threading.Tasks;

namespace EuroBooks.Application.Common.Interfaces
{
    public interface IEmailService
    {
        string SMTPServer { get; set; }
        string SMTPUserName { get; set; }
        string SMTPPassword { get; set; }
        string SMTPFromEmail { get; set; }
        string SMTPFromName { get; set; }
        int SMTPPort { get; set; }

        Task SendEmail(string To, string Subject, string HTMLMessage);
    }
}
