using EuroBooks.Application.Common.Interfaces;
using MediatR;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EuroBooks.Application.User.Commands
{
    /// <summary>
    /// Send user an email with link to reset password
    /// </summary>
    public class ResetUserPasswordEmailCommand : IRequest<bool>
    {
        public string Email { get; set; }

        public class ResetUserPasswordEmailCommandHandler : IRequestHandler<ResetUserPasswordEmailCommand, bool>
        {
            public readonly IEmailService emailService;

            public ResetUserPasswordEmailCommandHandler(IEmailService emailService)
            {
                this.emailService = emailService;
            }

            public async Task<bool> Handle(ResetUserPasswordEmailCommand request, CancellationToken cancellationToken)
            {
                // Get email template
                var template = $@"{Directory.GetCurrentDirectory()}\EmailTemplates\ResetPasswordTemplate.html";
                var templateString = File.ReadAllText(template);

                await emailService.SendEmail(request.Email, "Reset Password", templateString.ToString());

                return true;
            }
        }
    }
}
