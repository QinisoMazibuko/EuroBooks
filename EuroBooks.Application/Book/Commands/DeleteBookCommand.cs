using EuroBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EuroBooks.Application.Book.Commands
{
    public class DeleteBookCommand : IRequest<bool>
    {
        public long Id { get; set; }

        public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, bool>
        {
            private readonly IApplicationDbContext context;

            public DeleteBookCommandHandler(IApplicationDbContext context)
            {
                this.context = context;
            }

            public async Task<bool> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
            {
                // Validate Book
                var book = await context.Books.FirstOrDefaultAsync(v => v.Id == request.Id);
                if (book == null)
                    throw new Exception("Not a valid Book");

                book.IsActive = false;

                return await context.SaveChangesAsync(cancellationToken) > 0;
            }
        }
    }
}
