using AutoMapper;
using AutoMapper.QueryableExtensions;
using EuroBooks.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EuroBooks.Application.Book.Queries
{
    public class GetBookQuery : IRequest<BookDTO>
    {
        public long Id { get; set; }

        class GetBookQueryHandler : IRequestHandler<GetBookQuery, BookDTO>
        {
            public readonly IApplicationDbContext context;
            public readonly IMapper mapper;

            public GetBookQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                this.context = context;
                this.mapper = mapper;
            }

            public async Task<BookDTO> Handle(GetBookQuery request, CancellationToken cancellationToken)
            {
                var book = await context.Books
                    .Where(v => v.Id == request.Id)
                    .ProjectTo<BookDTO>(mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync();

                return book;
            }
        }
    }
}
