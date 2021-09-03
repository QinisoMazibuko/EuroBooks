using EuroBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EuroBooks.Application.Book.Commands
{
    public class UpdateBookCommand : IRequest<bool>
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public double PurchasePrice { get; set; }

        public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, bool>
        {
            private readonly IApplicationDbContext context;

            public UpdateBookCommandHandler(IApplicationDbContext context)
            {
                this.context = context;
            }

            public async Task<bool> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
            {
                //Validate Unique Name
                var uniqueName = await context.Books
                    .FirstOrDefaultAsync(x => x.Id != request.Id && x.Name.Trim().Equals(request.Name.Trim()));
                if (uniqueName != null)
                    throw new Exception("A Book with same name already exist");

                var book = await context.Books.FindAsync(request.Id);

                if (book != null)
                {
                    book.Name = request.Name;
                    book.Text = request.Text;
                    book.PurchasePrice = request.PurchasePrice;

                    return await context.SaveChangesAsync(cancellationToken) > 0;
                }

                throw new System.Exception("Could not find entity");
            }
        }
    }
}
