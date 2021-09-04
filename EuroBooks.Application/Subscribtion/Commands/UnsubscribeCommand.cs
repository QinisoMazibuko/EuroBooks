using EuroBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EuroBooks.Application.Subscribtion.Commands
{
    public class UnsubscribeCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public long UserId { get; set; }


        public class UnsubscribeCommandHandler : IRequestHandler<UnsubscribeCommand, bool>
        {
            private readonly IApplicationDbContext context;

            public UnsubscribeCommandHandler(IApplicationDbContext context)
            {
                this.context = context;
            }

            public async Task<bool> Handle(UnsubscribeCommand request, CancellationToken cancellationToken)
            {
                //Validate subscription is in the database
                var sub = await context.Subscriptions
                    .FirstOrDefaultAsync(x => x.Id != request.Id && x.UserID == request.UserId);
               
                if (sub == null)
                    throw new Exception("you are not subscribed to this Book");


                sub.IsActive = false;
                sub.LastModified = DateTime.Now;

                return await context.SaveChangesAsync(cancellationToken) > 0;
            }
        }
    }
}
