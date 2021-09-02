using EuroBooks.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EuroBooks.Application.User.Commands
{
    public class UpdatePasswordCommand : IRequest<bool>
    {
        public long UserID { get; set; }
        public string PasswordHash { get; set; }

        public class UpUpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, bool>
        {
            private readonly IIdentityService identityService;

            public UpUpdatePasswordCommandHandler(IIdentityService identityService)
            {
                this.identityService = identityService;
            }

            public async Task<bool> Handle(UpdatePasswordCommand request, CancellationToken cancellationToken)
            {
                var result = await identityService.UpdatePasswordAsync(request.UserID, request.PasswordHash);

                return result.Succeeded;
            }
        }
    }
}
