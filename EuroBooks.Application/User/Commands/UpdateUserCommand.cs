using EuroBooks.Application.Common.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EuroBooks.Application.User.Commands
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public long UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsActive { get; set; }
        public bool? IsDeleted { get; set; }

        public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
        {
            private readonly IIdentityService identityService;

            public UpdateUserCommandHandler(IIdentityService identityService)
            {
                this.identityService = identityService;
            }

            public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                var result = await identityService.UpdateUserAsync(request.UserId,
                    request.Email,
                    request.FirstName,
                    request.LastName,
                    request.IsActive,
                    request.IsDeleted);

                return result.Succeeded;
            }
        }
    }
}
