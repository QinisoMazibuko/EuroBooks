using AutoMapper;
using EuroBooks.Application.Common.Interfaces;
using MediatR;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EuroBooks.Application.Subscribtion.Queries
{
    public class GetUserSubscriptionsQuery : IRequest<List<SubscriptionDTO>>
    {
        public long UserId { get; set; }

        public class GetUserSubscriptionsQueryHandler : IRequestHandler<GetUserSubscriptionsQuery, List<SubscriptionDTO>>
        {
            public readonly IApplicationDbContext context;
            public readonly IMapper mapper;

            public GetUserSubscriptionsQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<List<SubscriptionDTO>> Handle(GetUserSubscriptionsQuery request, CancellationToken cancellationToken)
            {
                var subs = from rowS in context.Subscriptions
                            where rowS.IsActive &&
                            rowS.UserID == request.UserId
                            join rowB in context.Books
                            on rowS.BookID equals rowB.Id
                            select new SubscriptionDTO
                            {
                                Id = rowS.Id,
                                UserID = rowS.UserID,
                                BookID = rowS.BookID,
                                Book = new Book.BookDTO {
                                    Id = rowB.Id,
                                    Name = rowB.Name,
                                    Text = rowB.Text,
                                    PurchasePrice = rowB.PurchasePrice,
                                }
                            };

                var result = await subs.ToListAsync();

                return result;
            }
        }
    }
}
