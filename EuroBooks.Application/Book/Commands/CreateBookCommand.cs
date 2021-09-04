using EuroBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EuroBooks.Application.Book.Commands
{
    public class CreateBookCommand : IRequest<long>
    {
        public string Name { get; set; }
        public string Text { get; set; }
        public double PurchasePrice { get; set; }

        public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, long>
        {
            private readonly IApplicationDbContext context;

            public CreateBookCommandHandler(IApplicationDbContext context)
            {
                this.context = context;
            }

            public async Task<long> Handle(CreateBookCommand request, CancellationToken cancellationToken)
            {
                //Validate Unique Name
                var book = await context.Books.FirstOrDefaultAsync(x =>
                        x.Name.Equals(request.Name)
                        && x.Text == request.Text);

                if (book != null)
                    throw new Exception("A book with same name already exist");

                var newBook = new Domain.Enities.Book
                {
                    Name = request.Name,
                    Text = request.Text,
                    PurchasePrice = request.PurchasePrice,
                    IsActive = true,
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                };

                if (book == null)
                    context.Books.Add(newBook);

                await context.SaveChangesAsync(cancellationToken);

                return newBook.Id;
            }
        }
    }
}
