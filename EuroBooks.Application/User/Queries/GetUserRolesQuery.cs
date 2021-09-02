using AutoMapper;
using EuroBooks.Application.Common.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EuroBooks.Application.User.Queries
{
    public class GetUserRolesQuery : IRequest<List<string>>
    {
        class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, List<string>>
        {
            public readonly IMapper mapper;
            public readonly IIdentityService identityService;

            public GetUserRolesQueryHandler(IMapper mapper, IIdentityService identityService)
            {
                this.mapper = mapper;
                this.identityService = identityService;
            }

            public async Task<List<string>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
            {
                var userRoles = identityService.GetRoles().Where(x => x == "Admin" || x == "Subscriber").ToList();

                return userRoles;
            }
        }
    }
}
