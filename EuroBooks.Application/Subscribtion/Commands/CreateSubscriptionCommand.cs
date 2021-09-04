using EuroBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EuroBooks.Application.Subscribtion.Commands
{
    public class CreateSubscriptionCommand : IRequest<long>
    {
        public long UserId { get; set; }
        public long BookId { get; set; }

        public class CreateSubscriptionCommandHandler : IRequestHandler<CreateSubscriptionCommand, long>
        {
            private readonly IApplicationDbContext context;

            public CreateSubscriptionCommandHandler(IApplicationDbContext context)
            {
                this.context = context;
            }

            public async Task<long> Handle(CreateSubscriptionCommand request, CancellationToken cancellationToken)
            {
                //Validate there is no existing subsciption
                var sub = await context.Subscriptions.FirstOrDefaultAsync(x =>
                        x.UserID.Equals(request.UserId)
                        && x.BookID == request.BookId);

                if (sub != null)
                    throw new Exception("you are already subscribed to this book");

                var newSub = new Domain.Enities.Subscription
                {
                    UserID = request.UserId,
                    BookID = request.BookId,
                    CreatedBy = request.UserId,
                    IsActive = true,
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                };

                if (sub == null)
                    context.Subscriptions.Add(newSub);

                await context.SaveChangesAsync(cancellationToken);

                return newSub.Id;
            }
        }
    }
}
