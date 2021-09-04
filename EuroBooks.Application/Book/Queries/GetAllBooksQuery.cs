using AutoMapper;
using EuroBooks.Application.Common.Interfaces;
using MediatR;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EuroBooks.Application.Book.Queries
{
    public class GetAllBooksQuery : IRequest<List<BookDTO>>
    {
        public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery, List<BookDTO>>
        {
            public readonly IApplicationDbContext context;
            public readonly IMapper mapper;

            public GetAllBooksQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<List<BookDTO>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
            {
                var books = from row in context.Books
                            where row.IsActive
                            select new BookDTO
                            {
                                Id = row.Id,
                                Name = row.Name,
                                Text = row.Text,
                                PurchasePrice = row.PurchasePrice
                            };

                var result = await books.ToListAsync();
               

                return result;
            }
        }
    }
}
