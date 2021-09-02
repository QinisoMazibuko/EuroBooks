using EuroBooks.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EuroBooks.Application.User.Commands
{
    public class HashPasswordCommand : IRequest<string>
    {
        public long UserID { get; set; }
        public string Password { get; set; }

        public class HashPasswordCommandHandler : IRequestHandler<HashPasswordCommand, string>
        {
            private readonly IIdentityService identityService;

            public HashPasswordCommandHandler(IIdentityService identityService)
            {
                this.identityService = identityService;
            }

            public async Task<string> Handle(HashPasswordCommand request, CancellationToken cancellationToken)
            {
                var result = await identityService.HashPasswordAsync(request.UserID, request.Password);

                return result;
            }
        }
    }
}
